namespace Hitster.Networking.Packets;
//Packet das Sendet welcher Spieler der Host ist
public class HostPacket : Packet
{
    public int Player { get; }
    
    public HostPacket(int player) : base(PacketType.Host)
    {
        Player = player;
    }
}