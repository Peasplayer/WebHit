namespace HitsterServer.Packets;

public enum PacketType : uint
{
    Handshake,
    Track,
    Confirm,
    Move,
    SwitchTurn,
    Join,
    Start
}