using Fleck;
using HitsterServer.MusicData;
using HitsterServer.Packets;
using Newtonsoft.Json;

namespace HitsterServer;

public class GameServer
{
    public static GameServer Instance { get; private set; }

    public WebSocketServer Server { get; }
    public List<ClientData> Clients { get; }
    public int IdCounter { get; private set; }
    public bool GameIsStarted;
    public int CurrentPlayer { get; private set; }
    
    public GameServer(int port)
    {
        Instance = this;
        Clients = new List<ClientData>();
        
        Server = new WebSocketServer("ws://0.0.0.0:"  + port);
        Server.Start(connection =>
        {
            connection.OnOpen = () =>
            {
                Task.Run(() => OnConnect(connection));
            };
            
            connection.OnClose = () =>
            {
                Task.Run(() => OnDisconnect(connection));
            };

            connection.OnError = e =>
            {
                Task.Run(() => OnDisconnect(connection, e));
            };
            
            connection.OnMessage = msg =>
            {
                Task.Run(() => OnMessage(connection, msg));
            };
        });
    }

    private void OnConnect(IWebSocketConnection connection) {
        if (GameIsStarted || Clients.Count >= 6)
        {
            connection.Close();
            return;
        }

        var client = new ClientData(connection, IdCounter++);
        FleckLog.Info($"<{client.Id}> Connected");
        if (Clients.Count == 0)
        {
            client.IsHost = true;
            FleckLog.Info($"<{client.Id}> Became Host");
        }
        Clients.Add(client);
    }

    private void OnDisconnect(IWebSocketConnection connection, Exception? e = null) {
        var client = GetClient(connection);
        if (e != null)
            FleckLog.Error($"<{client.Id}> {e.Message}");
        FleckLog.Info($"<{client.Id}> Disconnected");
        Clients.Remove(client);
        SendPacketEveryone(new LeavePacket(client.Id));

        if (GameIsStarted && Clients.Count <= 1)
        {
            GameIsStarted = false;
            foreach (var c in Clients)
            {
                c.Connection.Close();
            }

            return;
        }
        
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

                GameIsStarted = true;
                MusicManager.ResetUsedTracks();

                SendPacketEveryone(rawPacket);
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
                await Task.Delay(5000);
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
            case PacketType.Token:
            {
                if (client.Id == CurrentPlayer)
                    return;
                
                var packet = JsonConvert.DeserializeObject<TokenPacket>(msg);
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
        }
    }

    private void SendPacket(Packet packet, params ClientData[] receivers)
    {
        var rawPacket = JsonConvert.SerializeObject(packet);
        foreach (var receiver in receivers)
        {
            receiver.Connection.Send(rawPacket);
        }
    }
    
    public void SendPacketEveryone(Packet packet)
    {
        var rawPacket = JsonConvert.SerializeObject(packet);
        foreach (var receiver in Clients)
        {
            receiver.Connection.Send(rawPacket);
        }
    }

    private ClientData GetClient(IWebSocketConnection connection)
    {
        var client = Clients.Find(c => connection.ConnectionInfo.Id == c.ConnId);
        if (client == null)
            throw new KeyNotFoundException("No client with that connection id found!");
        return client;
    }
}