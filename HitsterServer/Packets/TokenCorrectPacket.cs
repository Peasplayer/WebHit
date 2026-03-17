using HitsterServer.MusicData;
//Packet das gesendet wird wenn man durch das einsetzen eines Tokens ein Lied richtig errät und dieses bekommt

namespace HitsterServer.Packets;

public class TokenCorrectPacket : Packet
{
    public TrackData Track { get; }
    public int Id { get; }
    
    public TokenCorrectPacket(TrackData track, int id) : base(PacketType.TokenCorrect)
    {
        Track = track;
        Id = id;
    }
}