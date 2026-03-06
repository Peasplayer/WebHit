using System.Net.WebSockets;
using Hitster.Networking.Packets;
using Newtonsoft.Json;
using Websocket.Client;

namespace Hitster.Networking;

public class NetworkManager
{
    public static NetworkManager Instance { get; private set; }
    
    public WebsocketClient Client { get; }

    public NetworkManager(string address, string name)
    {
        Instance = this;
        Client = new WebsocketClient(new Uri(address));
        
        // Client wird konfiguriert
        Client.IsReconnectionEnabled = false;
        Client.ErrorReconnectTimeout = null;
        Client.ReconnectTimeout = TimeSpan.FromSeconds(5);
        
        // Nachrichten empfangen und an den Listener weiterleiten
        Client.MessageReceived.Subscribe(msg =>
        {
            if (msg.MessageType == WebSocketMessageType.Text && msg.Text != null)
            {
                // Nachricht wird in Task verarbeitet um den Empfänger-Thread nicht zu blockieren
                Task.Run(() => HandlePacket(msg.Text));
            }
        });
        
        // Wartet bis die Verbindung hergestellt wurde oder fehlschlägt
        Client.StartOrFail().Wait();

        SendPacket(new HandshakePacket(name));
    }

    private void HandlePacket(string msg)
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

                    Console.WriteLine($"Got name ({handshakePacket.Name}){(handshakePacket.IsHost ? " [Host]" : "")} assigned");
                    Player.LocalPlayer = new Player(handshakePacket.Id, handshakePacket.Name, handshakePacket.IsHost);
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
                    Player.Players.Find(p => p.Id == trackPacket.Id)?.PlaceCurrentTrack(0, trackPacket.Track);
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

                    if (joinPacket.Id == Player.LocalPlayer.Id)
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

                    Player.Players.RemoveAll(p => p.Id == leavePacket.Player);
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

                    Player.Players.Find(p => p.Id == hostPacket.Player)?.SetHost(true);
                    break;
                }
                case PacketType.Start:
                {
                    Lobby.Instance?.BeginInvoke(() =>
                    {
                        var form = new Form1();
                        form.Show();
                        Lobby.Instance.Hide();
                    });
                    break;
                }
                case PacketType.Confirm:
                {
                    Player.CurrentPlayer.ConfirmTrack();
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

                    Player.CurrentPlayer = Player.Players.Find(p => p.Id == turnPacket.Player);
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

                    Player.CurrentPlayer.PlaceCurrentTrack(movePacket.Index);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void SendPacket(Packet packet) {
        Client.Send(JsonConvert.SerializeObject(packet));
    }

    public void RpcStart()
    {
        SendPacket(new Packet(PacketType.Start));
    }
    
    public void RpcConfirmTrack()
    {
        if (Player.CurrentPlayer.Id != Player.LocalPlayer.Id)
            return;
        SendPacket(new Packet(PacketType.Confirm));
    }

    public void RpcMoveCurrentTrack(int index)
    {
        if (Player.CurrentPlayer.Id != Player.LocalPlayer.Id)
            return;
        SendPacket(new MovePacket(index));
    }
}