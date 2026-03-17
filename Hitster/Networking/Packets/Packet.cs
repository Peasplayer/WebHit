namespace Hitster.Networking.Packets;
//Grundstrucktur der Packets von dem alle Packets erben
public class Packet
{
    public PacketType PacketType { get; private set; }

    public Packet(PacketType packetType)
    {
        PacketType = packetType;
    }
}