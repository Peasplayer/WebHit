using System.Net.WebSockets;
using Hitster.Networking.Packets;
using Newtonsoft.Json;
using Websocket.Client;

namespace Hitster.Networking;

public class NetworkManager
{
    public static NetworkManager Instance { get; private set; }
    
    public WebsocketClient Client { get; private set; }
    public string Name { get; private set; }

    public NetworkManager()
    {
        Instance = this;
    }

    public void Connect(string address, string name)
    {
        Client = new WebsocketClient(new Uri(address));
        Name = name;
        
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

        SendPacket(new HandshakePacket(Name));
    }

    private void HandlePacket(string msg)
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
                break;
            }
        }
    }

    public void SendPacket(Packet packet) {
        Client.Send(JsonConvert.SerializeObject(packet));
    }
}