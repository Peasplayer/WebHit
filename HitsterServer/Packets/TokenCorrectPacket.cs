using HitsterServer.MusicData;

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