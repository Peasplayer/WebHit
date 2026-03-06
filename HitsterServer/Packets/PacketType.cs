namespace HitsterServer.Packets;

public enum PacketType : uint
{
    Handshake,
    Join,
    Host,
    Start,
    Track,
    Confirm,
    Move,
    SwitchTurn
}