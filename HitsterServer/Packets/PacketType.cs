namespace HitsterServer.Packets;

public enum PacketType : uint
{
    Handshake,
    RequestTrack,
    Track,
    Confirm,
    Move,
    SwitchTurn,
    Join
}