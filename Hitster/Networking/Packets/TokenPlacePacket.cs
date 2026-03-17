namespace Hitster.Networking.Packets;
//Packet für das plazieren eines Tokens. Es wird übergeben wer und wohin der Token plaziert wurde
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