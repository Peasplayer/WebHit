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

                    Console.WriteLine($"Got name ({handshakePacket.Name}) assigned");
                    Player.LocalPlayer = new Player(handshakePacket.Id, handshakePacket.Name);
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

                    Console.WriteLine($"Got song ({trackPacket.Track.Name})");
                    Player.Players.Find(p => p.Id == trackPacket.Id)?.PlaceCurrentTrack(0, trackPacket.Track);
                    //_track = trackPacket.Track;
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

                    Console.WriteLine($"Player {joinPacket.Name} ({joinPacket.Id}) joined");
                    new Player(joinPacket.Id, joinPacket.Name);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void SendPacket(Packet packet) {
        Client.Send(JsonConvert.SerializeObject(packet));
    }

    private TrackData? _track;
    public TrackData RequestTrackData()
    {
        var p = new Packet(PacketType.RequestTrack);
        SendPacket(p);
        while (_track == null) ;
        var track = _track;
        _track = null;
        return track;
    }
}