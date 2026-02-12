using HitsterServer.MusicData;

namespace HitsterServer.Packets;

public class TrackPacket : Packet
{
    public JsonStructs.TrackData Track { get; }
    
    public TrackPacket(JsonStructs.TrackData track) : base(PacketType.Track)
    {
        Track = track;
    }
}