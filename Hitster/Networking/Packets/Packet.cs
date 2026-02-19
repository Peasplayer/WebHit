namespace Hitster.Networking.Packets;

public class Packet
{
    public string ConversationId { get; private set; }
    public PacketType PacketType { get; private set; }

    public Packet(PacketType packetType, string? conversationId)
    {
        PacketType = packetType;
        ConversationId = conversationId ?? Guid.NewGuid().ToString();
    }
}