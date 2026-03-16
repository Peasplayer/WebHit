namespace Hitster.Networking.Packets;

public class DisconnectPacket : Packet
{
    public string Message { get; }

    public DisconnectPacket(string message) : base(PacketType.Disconnect)
    {
        Message = message;
    }
}