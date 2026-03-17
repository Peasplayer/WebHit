using Fleck;
using HitsterServer.MusicData;
using HitsterServer.Packets;
using Newtonsoft.Json;

namespace HitsterServer;

public class GameServer
{
    public static GameServer Instance { get; private set; }

    private WebSocketServer Server { get; } //Der Websocket Server
    private List<ClientData> Clients { get; } //Liste aller verbundenen Spieler
    private int IdCounter { get; set; } //Zählt hoch um jedem Spieler eine eindeutige ID zu geben
    private bool GameIsStarted; //Gibt an ob das Spiel bereits gestartet ist
    private int CurrentPlayer { get; set; } //ID des Speilers der gerade am zug ist
    
    public GameServer(int port)
    {
        Instance = this;
        Clients = new List<ClientData>();
        
        Server = new WebSocketServer("ws://0.0.0.0:"  + port); //Starten der Websocket-Schnittstelle
        Server.Start(connection =>
        {
            connection.OnOpen = () =>
            {
                Task.Run(() => OnConnect(connection)); //Wenn ein neuer Client sich verbindet
            };
            
            connection.OnClose = () =>
            {
                Task.Run(() => OnDisconnect(connection)); //Wenn ein Client die verbindung abbricht 
            };

            connection.OnError = e =>
            {
                Task.Run(() => OnDisconnect(connection, e)); //Wenn die verbidnung durch eine Fehler abgebrochen wird
            };
            
            connection.OnMessage = msg =>
            {
                Task.Run(() => OnMessage(connection, msg)); //Wenn der Client eine nachricht an den Server schickt
            };
        });
    }

    private void OnConnect(IWebSocketConnection connection) {
        //Wenn das Spiel gestartet ist kann niemand mehr beitreten
        if (GameIsStarted)
        {
            SendPacketEveryone(new DisconnectPacket("Das Spiel hat bereits begonnen!"));
            return;
        }

        //Maximal 6 Spieler dürfen beitreten
        if (Clients.Count >= 6)
        {
            SendPacketEveryone(new DisconnectPacket("Das Spiel ist bereits voll!"));
            return;
        }

        //Neuer Client erstellen und in die Liste speichern
        var client = new ClientData(connection, IdCounter++);
        FleckLog.Info($"<{client.Id}> Connected");
        //Der erste Spieler im Server wird Host
        if (Clients.Count == 0)
        {
            client.IsHost = true;
            FleckLog.Info($"<{client.Id}> Became Host");
        }
        Clients.Add(client);
    }

    //Wird aufgerufen wenn jemadn das Spiel verläst
    private void OnDisconnect(IWebSocketConnection connection, Exception? e = null) {
        var client = GetClient(connection);
        if (e != null)
            FleckLog.Error($"<{client.Id}> {e.Message}");
        FleckLog.Info($"<{client.Id}> Disconnected");
        Clients.Remove(client); //Spieler aus Liste entfernen
        SendPacketEveryone(new LeavePacket(client.Id)); // Allen mitteilen, dass der Spieler weg ist

        // Hat das Spiel bereits begonnen, wird es beendet
        if (GameIsStarted)
        {
            GameIsStarted = false;
            SendPacketEveryone(new DisconnectPacket("Ein Spieler hat die Runde beendet!"));

            return;
        }
        
        //Wenn der Host in der Lobby das Spiel verlässt, wird der nächste Spieler Host
        if (client.IsHost && Clients.Count != 0)
        {
            Clients[0].IsHost = true;
            FleckLog.Info($"<{Clients[0].Id}> Became Host");
            SendPacketEveryone(new HostPacket(Clients[0].Id));
        }
    }

    private async void OnMessage(IWebSocketConnection connection, string msg)
    {
        var client = GetClient(connection);
        FleckLog.Info($"<{client.Id}> Said: {msg}");
        
        var rawPacket = JsonConvert.DeserializeObject<Packet>(msg);
        if (rawPacket == null)
        {
            FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
            return;
        }

        // Verarbeitung der verschiedenen JSON-Packets
        switch (rawPacket.PacketType)
        {
            case PacketType.Handshake:
            {
                var packet = JsonConvert.DeserializeObject<HandshakePacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }

                // Falls der Name schon vergeben ist, wird eine Zahl dran gehängt
                var name = packet.Name;
                var iteration = 0;
                while (Clients.Find(c => c.Name == name) != null)
                {
                    iteration++;
                    name = packet.Name + " (" + iteration + ")";
                }

                client.Name = name;

                SendPacket(new HandshakePacket(name, client.Id, client.IsHost), client);

                Task.Delay(500).Wait();
                // Spieler wird allen bekanntgegeben
                foreach (var c in Clients)
                {
                    if (c.Id != client.Id && c.Name != null)
                        SendPacket(new JoinPacket(c.Name, c.Id, c.IsHost), client);
                }
                SendPacketEveryone(new JoinPacket(name, client.Id, client.IsHost));

                FleckLog.Info($"<{client.Id}> Name set to: {name}");
                break;
            }
            case PacketType.Start:
            {
                if (!client.IsHost)
                    return;
                
                var packet = JsonConvert.DeserializeObject<StartPacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }

                Settings.CurrentSettings = packet.Settings;
                GameIsStarted = true;
                MusicManager.ResetUsedTracks();

                SendPacketEveryone(packet);
                foreach (var c in Clients)
                {
                    SendPacketEveryone(new TrackPacket(await MusicManager.GetRandomTrack(), c.Id));
                }
                
                CurrentPlayer = Clients[0].Id;
                SendPacketEveryone(new TurnPacket(CurrentPlayer));
                SendPacketEveryone(new TrackPacket(await MusicManager.GetRandomTrack(), CurrentPlayer));
                break;
            }
            case PacketType.Confirm:
            {
                if (client.Id != CurrentPlayer)
                    return;
                
                SendPacketEveryone(rawPacket);
                await Task.Delay(Settings.CurrentSettings.TokenPlaceTime * 1000);
                SendPacketEveryone(new Packet(PacketType.Reveal));
                await Task.Delay(5000);

                if (!GameIsStarted)
                    return;

                var index = Clients.IndexOf(client);
                CurrentPlayer = (index + 1 >= Clients.Count ? Clients[0] : Clients[index + 1]).Id;
                SendPacketEveryone(new TurnPacket(CurrentPlayer));
                SendPacketEveryone(new TrackPacket(await MusicManager.GetRandomTrack(), CurrentPlayer));
                break;
            }
            case PacketType.TokenAdd:
            {
                var packet = JsonConvert.DeserializeObject<TokenAddPacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }
                
                SendPacketEveryone(packet);
                break;
            }
            case PacketType.TokenPlace:
            {
                if (client.Id == CurrentPlayer)
                    return;
                
                var packet = JsonConvert.DeserializeObject<TokenPlacePacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }
                
                SendPacketEveryone(packet);
                break;
            }
            case PacketType.TokenCorrect:
            {
                var packet = JsonConvert.DeserializeObject<TokenCorrectPacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }

                SendPacketEveryone(packet);
                break;
            }
            case PacketType.Move:
            {
                if (client.Id != CurrentPlayer)
                    return;
                
                var packet = JsonConvert.DeserializeObject<MovePacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }
                
                SendPacketEveryone(packet);
                break;
            }
            case PacketType.Win:
            {
                var packet = JsonConvert.DeserializeObject<WinPacket>(msg);
                if (packet == null)
                {
                    FleckLog.Warn($"<{client.Id}> Malformed Packet received!");
                    return;
                }
                
                SendPacketEveryone(packet);
                GameIsStarted = false;
                foreach (var c in Clients)
                    c.Connection.Close();
                break;
            }
            case PacketType.SkipTrack:
            {
                if (client.Id != CurrentPlayer)
                {
                    return;
                }
                SendPacketEveryone(new Packet(PacketType.SkipTrack));
                SendPacketEveryone(new TokenAddPacket(client.Id, -1));
                var newTrack = await MusicManager.GetRandomTrack();
                SendPacketEveryone(new TrackPacket(newTrack, client.Id));
                break;
            }
            case PacketType.BuyTrack:
            {
                //Drei Tokesn abziehn
                SendPacketEveryone(new TokenAddPacket(client.Id, -3));
                //Neue Karte dem Spieler geben
                var newTrack = await MusicManager.GetRandomTrack();
                //Damit die Karte direkt umgedrehht auf die Timeline gelegt wird
                SendPacketEveryone(new TokenCorrectPacket(newTrack, client.Id)); 
                break;
            }
        }
    }

    //Sendet Packets an bestimmte Personen
    private void SendPacket(Packet packet, params ClientData[] receivers)
    {
        var rawPacket = JsonConvert.SerializeObject(packet);
        foreach (var receiver in receivers)
        {
            receiver.Connection.Send(rawPacket);
        }
    }
    
    //Sendet Packets an jeden
    public void SendPacketEveryone(Packet packet)
    {
        var rawPacket = JsonConvert.SerializeObject(packet);
        foreach (var receiver in Clients)
        {
            receiver.Connection.Send(rawPacket);
        }
    }

    //Scuht das passende Client Objekt
    private ClientData GetClient(IWebSocketConnection connection)
    {
        var client = Clients.Find(c => connection.ConnectionInfo.Id == c.ConnId);
        if (client == null)
            throw new KeyNotFoundException("No client with that connection id found!");
        return client;
    }
}