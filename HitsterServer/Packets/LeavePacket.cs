namespace HitsterServer.Packets;

public class LeavePacket : Packet
{
    public int Player { get; }
    
    public LeavePacket(int player) : base(PacketType.Leave)
    {
        Player = player;
    }
}