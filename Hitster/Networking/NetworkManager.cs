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
                Console.WriteLine("MESSAGE:" + msg.Text);
                // Nachricht wird in Packet umgewandelt
                var packet = JsonConvert.DeserializeObject<Packet>(msg.Text);
                if (packet == null)
                {
                    Console.WriteLine("Received malformed packet!");
                    return;
                }
                
                // Packet wird in Task verarbeitet um den Empfänger-Thread nicht zu blockieren
                //Task.Run(() => HandlePacket(packet));
            }
        });
        
        // Wartet bis die Verbindung hergestellt wurde oder fehlschlägt
        Client.StartOrFail().Wait();

        SendPacket(new HandshakePacket(Name));
    }

    public void SendPacket(Packet packet) {
        Client.Send(JsonConvert.SerializeObject(packet));
    }
}