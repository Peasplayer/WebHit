namespace HitsterServer.Packets;

public class HandshakePacket : Packet
{
    public string Name { get; }
    
    public HandshakePacket(string name) : base(PacketType.Handshake)
    {
        Name = name;
    }
}