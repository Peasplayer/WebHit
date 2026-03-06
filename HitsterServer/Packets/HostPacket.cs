namespace HitsterServer.Packets;

public class HostPacket : Packet
{
    public int Player { get; }
    
    public HostPacket(int player) : base(PacketType.Host)
    {
        Player = player;
    }
}