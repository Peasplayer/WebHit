namespace Hitster.Networking.Packets;
//Packet das gesendet wird wenn ein Spieler gewonnen hat
public class WinPacket : Packet
{
    public int Player { get; }
    
    public WinPacket(int player) : base(PacketType.Win)
    {
        Player = player;
    }
}