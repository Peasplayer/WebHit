namespace HitsterServer.Packets;

public enum PacketType : uint
{
    Handshake,
    Disconnect,
    Join,
    Leave,
    Host,
    Start,
    Track,
    Confirm,
    Move,
    SwitchTurn,
    TokenAdd,
    TokenPlace,
    TokenCorrect,
    Reveal,
    Win,
    SkipTrack,
    BuyTrack
}