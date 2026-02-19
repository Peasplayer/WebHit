namespace Hitster.Networking.Packets;

public class TrackPacket : Packet
{
    public TrackData Track { get; }
    
    public TrackPacket(TrackData track, string? conversationId = null) : base(PacketType.Track, conversationId)
    {
        Track = track;
    }
}