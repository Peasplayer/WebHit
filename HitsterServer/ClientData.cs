using Fleck;

namespace HitsterServer;

public class ClientData
{
    public IWebSocketConnection Connection { get; } //Websocket Verbindung des Clients
    public int Id { get; private set; } //Eindeutige ID des Clients
    public string? Name; //Der Name des Clients
    public bool IsHost; //Gibt an ob der Client auch der Host ist

    public ClientData(IWebSocketConnection connection, int id, string? name = null, bool isHost = false)
    {
        Connection = connection;
        Id = id;
        Name = name;
        IsHost = isHost;
    }
}