namespace Hitster.Networking.Packets;
// Packet zur Eröffnung des Servers bei dem Name, Id, und ob die Person der Host ist übergeben wird
public class HandshakePacket : Packet
{
    public string Name { get; }
    public int Id { get; }
    public bool IsHost { get; }

    public HandshakePacket(string name, int id = -1, bool isHost = false) : base(PacketType.Handshake)
    {
        Name = name;
        Id = id;
        IsHost = isHost;
    }
}