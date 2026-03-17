namespace Hitster.Networking.Packets;
//Packet welcher Spieler jetzt am Zug ist
public class TurnPacket : Packet
{
    public int Player { get; }

    public TurnPacket(int player) : base(PacketType.SwitchTurn)
    {
        Player = player;
    }
}