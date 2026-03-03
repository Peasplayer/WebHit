using Fleck;

namespace HitsterServer;

public class ClientData
{
    public IWebSocketConnection Connection { get; } //Websocket Verbindung des Clients
    public int Id { get; private set; } //Eindeutige ID des Clients
    public Guid ConnId { get; private set; } // ID der passenden Verbindung
    public string? Name; //Der Name des Clients
    public bool IsHost; //Gibt an ob der Client auch der Host ist

    public ClientData(IWebSocketConnection connection, int id, string? name = null, bool isHost = false)
    {
        Connection = connection;
        Id = id;
        ConnId = Connection.ConnectionInfo.Id;
        Name = name;
        IsHost = isHost;
    }
}