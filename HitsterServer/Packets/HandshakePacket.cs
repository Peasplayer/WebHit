namespace HitsterServer.Packets;

public class HandshakePacket : Packet
{
    public string Name { get; }
    
    public HandshakePacket(string name, string? conversationId) : base(PacketType.Handshake, conversationId)
    {
        Name = name;
    }
}