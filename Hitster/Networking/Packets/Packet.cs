namespace Hitster.Networking.Packets;

public class Packet
{
    public PacketType PacketType { get; private set; }

    public Packet(PacketType packetType)
    {
        PacketType = packetType;
    }
}