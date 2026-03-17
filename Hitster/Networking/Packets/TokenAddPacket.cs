namespace Hitster.Networking.Packets;
//Packet das gesendet wird wenn man Token verdint oder ausgibt mit Spieler und Anzahl der Token-Differenz
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