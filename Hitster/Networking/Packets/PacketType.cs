namespace Hitster.Networking.Packets;

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
    SwitchTurn,
    TokenAdd,
    TokenPlace,
    TokenCorrect,
    Reveal,
    Win,
    SkipTrack,
    BuyTrack
}