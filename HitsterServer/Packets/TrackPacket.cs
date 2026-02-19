using HitsterServer.MusicData;

namespace HitsterServer.Packets;

public class TrackPacket : Packet
{
    public TrackData Track { get; }
    
    public TrackPacket(TrackData track, string? conversationId) : base(PacketType.Track, conversationId)
    {
        Track = track;
    }
}