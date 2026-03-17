namespace Hitster.Networking.Packets;
//Packet zum beitreten des Servers
public class JoinPacket : Packet
{
    public string Name { get; }
    public int Id { get; }
    public bool IsHost { get; }
    
    public JoinPacket(string name, int id, bool isHost) : base(PacketType.Join)
    {
        Name = name;
        Id = id;
        IsHost = isHost;
    }
}