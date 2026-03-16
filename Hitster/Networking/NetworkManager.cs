using System.Net.WebSockets;
using Hitster.Networking.Packets;
using Newtonsoft.Json;
using Websocket.Client;

namespace Hitster.Networking;

public class NetworkManager
{
    private static WebsocketClient? _client;
    private static bool _normalDisconnect;

    public static void Connect(string address, string name)
    {
        _client = new WebsocketClient(new Uri(address));
        _normalDisconnect = false;
        
        // Client wird konfiguriert
        _client.IsReconnectionEnabled = false;
        _client.ErrorReconnectTimeout = null;
        _client.ReconnectTimeout = TimeSpan.FromSeconds(5);
        _client.DisconnectionHappened.Subscribe(info =>
        {
            Form1.CloseForm();
            Lobby.CloseForm();
            MenuForm.ShowForm();
            if (!_normalDisconnect)
                MessageBox.Show("Die Verbindung mit dem Server wurde getrennt!");
        });
        
        // Nachrichten empfangen und an den Listener weiterleiten
        _client.MessageReceived.Subscribe(msg =>
        {
            if (msg.MessageType == WebSocketMessageType.Text && msg.Text != null)
            {
                // Nachricht wird in Task verarbeitet um den Empfänger-Thread nicht zu blockieren
                Task.Run(() => HandlePacket(msg.Text));
            }
        });
        
        // Wartet bis die Verbindung hergestellt wurde oder fehlschlägt
        _client.StartOrFail().Wait();

        SendPacket(new HandshakePacket(name));
    }

    public static void Disconnect()
    {
        _client?.Dispose();
    }

    private static void HandlePacket(string msg)
    {
        try
        {
            Console.WriteLine("MESSAGE:" + msg);

            // Nachricht wird in Packet umgewandelt
            var packet = JsonConvert.DeserializeObject<Packet>(msg);
            if (packet == null)
            {
                Console.WriteLine("Received malformed packet!");
                return;
            }

            switch (packet.PacketType)
            {
                case PacketType.Handshake:
                {
                    var handshakePacket = JsonConvert.DeserializeObject<HandshakePacket>(msg);
                    if (handshakePacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Console.WriteLine($"Got name ({handshakePacket.Name})[{handshakePacket.Id}]{(handshakePacket.IsHost ? " [Host]" : "")} assigned");
                    Player.SetLocalPlayer(new Player(handshakePacket.Id, handshakePacket.Name, handshakePacket.IsHost));
                    break;
                }
                case PacketType.Disconnect:
                {
                    var disconnectPacket = JsonConvert.DeserializeObject<DisconnectPacket>(msg);
                    if (disconnectPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }
                    
                    MessageBox.Show(disconnectPacket.Message, "Achtung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _normalDisconnect = true;
                    _client?.Dispose();
                    
                    break;
                }
                case PacketType.Track:
                {
                    var trackPacket = JsonConvert.DeserializeObject<TrackPacket>(msg);
                    if (trackPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Console.WriteLine($"Got song ({trackPacket.Track.Name}) by ({trackPacket.Track.Artist})");
                    Player.GetPlayer(trackPacket.Id).PlaceCurrentTrack(0, trackPacket.Track);
                    break;
                }
                case PacketType.Join:
                {
                    var joinPacket = JsonConvert.DeserializeObject<JoinPacket>(msg);
                    if (joinPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    if (joinPacket.Id == Player.LocalPlayer.Id || Player.AllPlayers.Find(p => p.Id == joinPacket.Id) != null)
                        return;

                    Console.WriteLine($"Player {joinPacket.Name} ({joinPacket.Id}){(joinPacket.IsHost ? " [Host]" : "")} joined");
                    new Player(joinPacket.Id, joinPacket.Name, joinPacket.IsHost);
                    break;
                }
                case PacketType.Leave:
                {
                    var leavePacket = JsonConvert.DeserializeObject<LeavePacket>(msg);
                    if (leavePacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Player.RemovePlayer(Player.GetPlayer(leavePacket.Player));
                    break;
                }
                case PacketType.Host:
                {
                    var hostPacket = JsonConvert.DeserializeObject<HostPacket>(msg);
                    if (hostPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Player.GetPlayer(hostPacket.Player).SetHost(true);
                    break;
                }
                case PacketType.Start:
                {
              
                    var startPacket = JsonConvert.DeserializeObject<StartPacket>(msg);
                    if (startPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Settings.CurrentSettings = startPacket.Settings;
                    //Spielform wird aufgerufen
                    Lobby.OpenGameForm();
                    break;
                }
                case PacketType.Confirm:
                {
                    //Ein Spieler hat sein Lied bestätigt
                    Form1.StopTimer();
                    Form1.StartTimer("Token Platzieren", Settings.CurrentSettings.TokenPlaceTime);
                    Player.TokenGuesses.Clear();
                    Timeline.ToggleTokenPlacement(true);
                    break;
                }
                case PacketType.TokenPlace:
                {
                    var tokenPacket = JsonConvert.DeserializeObject<TokenPlacePacket>(msg);
                    if (tokenPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    if (!Player.TokenGuesses.ContainsKey(tokenPacket.Id))
                    {
                        Player.TokenGuesses.Add(tokenPacket.Id, tokenPacket.Index);
                        Player.GetPlayer(tokenPacket.Id).AddTokens(-1);
                        Timeline.UpdateTimeline(Player.CurrentPlayer);
                    }
                    break;
                }
                case PacketType.TokenCorrect:
                {
                    var trackPacket = JsonConvert.DeserializeObject<TrackPacket>(msg);
                    if (trackPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Player.GetPlayer(trackPacket.Id).AddTrack(trackPacket.Track);
                    break;
                }
                case PacketType.TokenAdd:
                {
                    var tokenPacket = JsonConvert.DeserializeObject<TokenAddPacket>(msg);
                    if (tokenPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Player.GetPlayer(tokenPacket.Id).AddTokens(tokenPacket.Amount);
                    break;
                }
                case PacketType.Reveal:
                {
                    //Aufdecken des aktuellen Songs
                    Form1.StopTimer();
                    Timeline.ToggleTokenPlacement(false); //Tokens können nicht mehr gesetzt werden
                    Player.CurrentPlayer?.RevealCurrentTrack();
                    break;
                }
                case PacketType.SwitchTurn:
                {
                    var turnPacket = JsonConvert.DeserializeObject<TurnPacket>(msg);
                    if (turnPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Player.SetCurrentPlayer(Player.GetPlayer(turnPacket.Player));
                    if (Player.CurrentPlayer != Player.LocalPlayer)
                        Form1.SetOtherTimeline(Player.CurrentPlayer);
                    break;
                }
                case PacketType.Move:
                {
                    var movePacket = JsonConvert.DeserializeObject<MovePacket>(msg);
                    if (movePacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Player.CurrentPlayer?.PlaceCurrentTrack(movePacket.Index);
                    break;
                }
                case PacketType.Win:
                {
                    var winPacket = JsonConvert.DeserializeObject<WinPacket>(msg);
                    if (winPacket == null)
                    {
                        Console.WriteLine("Received malformed packet!");
                        return;
                    }

                    Form1.PlayerWon(Player.GetPlayer(winPacket.Player));
                    break;
                }
                case PacketType.SkipTrack:
                {
                    Player.CurrentPlayer?.DiscardCurrentTrack();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void SendPacket(Packet packet)
    {
        _client?.Send(JsonConvert.SerializeObject(packet));
    }

    public static void RpcStart()
    {
        SendPacket(new StartPacket(Settings.CurrentSettings));
    }
    
    public static void RpcConfirmTrack()
    {
        if (Player.CurrentPlayer.Id != Player.LocalPlayer.Id)
            return;
        SendPacket(new Packet(PacketType.Confirm));
    }

    public static void RpcMoveCurrentTrack(int index)
    {
        if (Player.CurrentPlayer.Id != Player.LocalPlayer.Id)
            return;
        SendPacket(new MovePacket(index));
    }

    public static void RpcPlayerWon(Player player)
    {
        SendPacket(new WinPacket(player.Id));
    }

    public static void RpcPlaceToken(int index)
    {
        if (Player.CurrentPlayer?.Id == Player.LocalPlayer.Id)
            return;
        SendPacket(new TokenPlacePacket(Player.LocalPlayer.Id, index));
    }

    public static void RpcTokenCorrect(Player player, TrackData track)
    {
        SendPacket(new TokenCorrectPacket(track, player.Id));
    }

    public static void RpcAddToken(int id, int amount)
    {
        SendPacket(new TokenAddPacket(id, amount));
    }

    public static void RpcSkipTrack()
    {
        if (Player.CurrentPlayer?.Id != Player.LocalPlayer?.Id || Player.LocalPlayer?.Tokens < 1)
            return;
        SendPacket(new Packet(PacketType.SkipTrack));
    }
    
    public static void RpcBuyTrack()
    {
        if (Player.LocalPlayer?.Tokens < 3)
            return;
        SendPacket(new Packet(PacketType.BuyTrack));
    }
}