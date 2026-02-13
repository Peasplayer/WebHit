namespace Hitster.Networking.Packets;

public class TrackPacket : Packet
{
    public TrackData Track { get; }
    
    public TrackPacket(TrackData track) : base(PacketType.Track)
    {
        Track = track;
    }
}