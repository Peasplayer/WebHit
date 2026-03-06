namespace HitsterServer.Packets;

public class HandshakePacket : Packet
{
    public string Name { get; }
    public int Id { get; }
    public bool IsHost { get; }
    
    public HandshakePacket(string name, int id, bool isHost) : base(PacketType.Handshake)
    {
        Name = name;
        Id = id;
        IsHost = isHost;
    }
}