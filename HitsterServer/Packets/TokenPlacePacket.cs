namespace HitsterServer.Packets;

public class TokenPlacePacket : Packet
{
    public int Id { get; }
    public int Index { get; }
    
    public TokenPlacePacket(int id, int index) : base(PacketType.TokenPlace)
    {
        Id = id;
        Index = index;
    }
}