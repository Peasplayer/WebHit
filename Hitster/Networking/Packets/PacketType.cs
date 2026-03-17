namespace Hitster.Networking.Packets;
//Alle Packet-Typen bzw. deren IDs
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