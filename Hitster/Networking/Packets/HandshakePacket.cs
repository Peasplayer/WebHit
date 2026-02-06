namespace Hitster.Networking.Packets;

public class HandshakePacket : Packet
{
    public string Name { get; }

    public HandshakePacket(string name)
    {
        Name = name;
    }
}