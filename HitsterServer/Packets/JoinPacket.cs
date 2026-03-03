namespace HitsterServer.Packets;

public class JoinPacket : Packet
{
    public string Name { get; }
    public int Id { get; }
    
    public JoinPacket(string name, int id) : base(PacketType.Join)
    {
        Name = name;
        Id = id;
    }
}