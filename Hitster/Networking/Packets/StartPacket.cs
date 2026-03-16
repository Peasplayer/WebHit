namespace Hitster.Networking.Packets;

public class StartPacket : Packet
{
    public Settings Settings { get; private set; }

    public StartPacket(Settings settings) : base(PacketType.Start)
    {
        Settings = settings;
    }
}