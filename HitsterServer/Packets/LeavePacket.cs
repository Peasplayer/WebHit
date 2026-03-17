namespace HitsterServer.Packets;
//Packet beim verlassen des Servers

public class LeavePacket : Packet
{
    public int Player { get; }
    
    public LeavePacket(int player) : base(PacketType.Leave)
    {
        Player = player;
    }
}