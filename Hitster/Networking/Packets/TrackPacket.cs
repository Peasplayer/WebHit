namespace Hitster.Networking.Packets;
//Packet mit dem Lied und wer das Lied erhalten hat
public class TrackPacket : Packet
{
    public TrackData Track { get; }
    public int Id { get; }
    
    public TrackPacket(TrackData track, int id) : base(PacketType.Track)
    {
        Track = track;
        Id = id;
    }
}