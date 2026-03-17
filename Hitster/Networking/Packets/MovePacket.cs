namespace Hitster.Networking.Packets;
//Packet das gesendet wird wenn die Karte bewegt wird
public class MovePacket : Packet
{
    public int Index { get; }
    
    public MovePacket(int index) : base(PacketType.Move)
    {
        Index = index;
    }
}