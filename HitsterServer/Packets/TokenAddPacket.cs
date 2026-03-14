namespace HitsterServer.Packets;

public class TokenAddPacket : Packet
{
    public int Id { get; }
    public int Amount { get; }
    
    public TokenAddPacket(int id, int amount) : base(PacketType.TokenAdd)
    {
        Id = id;
        Amount = amount;
    }
}