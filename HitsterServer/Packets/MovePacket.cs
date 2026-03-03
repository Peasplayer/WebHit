namespace HitsterServer.Packets;

public class MovePacket : Packet
{
    public int Index { get; }

    public MovePacket(int index) : base(PacketType.Move)
    {
        Index = index;
    }
}