namespace HitsterServer.Packets;

public class HandshakePacket : Packet
{
    public override PacketType PacketType => PacketType.Handshake;
    
    public string Name { get; }
    
    public HandshakePacket(string name)
    {
        Name = name;
    }
}