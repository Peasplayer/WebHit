namespace Hitster.Networking.Packets;
//Packet welches gesendet wird wenn der Client die Verbindung trennen soll mit Begründung
public class DisconnectPacket : Packet
{
    public string Message { get; }

    public DisconnectPacket(string message) : base(PacketType.Disconnect)
    {
        Message = message;
    }
}