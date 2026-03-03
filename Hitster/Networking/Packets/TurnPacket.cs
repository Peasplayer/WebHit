namespace Hitster.Networking.Packets;

public class TurnPacket : Packet
{
    public int Player { get; }

    public TurnPacket(int player) : base(PacketType.SwitchTurn)
    {
        Player = player;
    }
}