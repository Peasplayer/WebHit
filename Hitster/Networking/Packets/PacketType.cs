namespace Hitster.Networking.Packets;

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