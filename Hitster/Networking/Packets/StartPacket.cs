namespace Hitster.Networking.Packets;
//Packet das gesendet wird wenn das Spiel startet, bei dem die verwendeten Einstellungen übergeben werden
public class StartPacket : Packet
{
    public Settings Settings { get; private set; }

    public StartPacket(Settings settings) : base(PacketType.Start)
    {
        Settings = settings;
    }
}