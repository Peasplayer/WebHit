using Fleck;
using HitsterServer.MusicData;
using HitsterServer.Packets;
using Newtonsoft.Json;

namespace HitsterServer;

public class GameServer
{
    public static GameServer Instance { get; private set; }

    public WebSocketServer Server { get; }
    public Dictionary<string, ClientData> Clients { get; }
    public int IdCounter { get; private set; }
    public bool GameIsStarted;
    
    public GameServer(int port)
    {
        Instance = this;
        Clients = new Dictionary<string, ClientData>();
        
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
        var client = new ClientData(connection, IdCounter++);
        FleckLog.Info($"<{client.Id}> Connected");
        Clients.Add(connection.ConnectionInfo.Id.ToString(), client);
    }

    private void OnDisconnect(IWebSocketConnection connection, Exception? e = null) {
        var client = Clients[connection.ConnectionInfo.Id.ToString()];
        if (e != null) {
            FleckLog.Error($"<{client.Id}> {e.Message}");
        }
        FleckLog.Info($"<{client.Id}> Disconnected");
        Clients.Remove(connection.ConnectionInfo.Id.ToString());
    }

    private async void OnMessage(IWebSocketConnection connection, string msg)
    {
        var client = Clients[connection.ConnectionInfo.Id.ToString()];
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
                while (Clients.Values.ToList().Find(c => c.Name == name) != null)
                {
                    iteration++;
                    name = packet.Name + " (" + iteration + ")";
                }
                
                client.Name = name;
                
                SendPacket(new HandshakePacket(name, rawPacket.ConversationId), client);
                
                FleckLog.Info($"<{client.Id}> Name set to: {name}");
                break;
            }
            case PacketType.RequestTrack:
            {
                // Bereit Antwort-Packet mit zufälligem Song vor
                var track = await MusicManager.GetRandomTrack();
                var response = new TrackPacket(track, rawPacket.ConversationId);
                
                // Jeder kriegt den Song um an der Runde teilzuhaben
                SendPacketEveryone(response);
                
                FleckLog.Info($"<{client.Id}> got song '{track.Name}' ({track.ReleaseYear})");
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
    
    private void SendPacketEveryone(Packet packet)
    {
        var rawPacket = JsonConvert.SerializeObject(packet);
        foreach (var receiver in Clients.Values)
        {
            receiver.Connection.Send(rawPacket);
        }
    }
}