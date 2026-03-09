namespace Hitster.Networking.Packets;

public class WinPacket : Packet
{
    public int Player { get; }
    
    public WinPacket(int player) : base(PacketType.Win)
    {
        Player = player;
    }
}