namespace Hitster.Networking.Packets;

public class HandshakePacket : Packet
{
    public string Name { get; }

    public HandshakePacket(string name, string? conversationId = null) : base(PacketType.Handshake, conversationId)
    {
        Name = name;
    }
}