namespace HitsterServer.Packets;

public enum PacketType : uint
{
    Handshake,
    Join,
    Leave,
    Host,
    Start,
    Track,
    Confirm,
    Move,
    SwitchTurn
}