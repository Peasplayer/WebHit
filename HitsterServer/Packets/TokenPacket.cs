namespace HitsterServer.Packets;

public class TokenPacket : Packet
{
    public int Id { get; }
    public int Index { get; }
    
    public TokenPacket(int id, int index) : base(PacketType.Token)
    {
        Id = id;
        Index = index;
    }
}