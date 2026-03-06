namespace Hitster.Networking.Packets;

public class HandshakePacket : Packet
{
    public string Name { get; }
    public int Id { get; }
    public bool IsHost { get; }

    public HandshakePacket(string name, int id = -1, bool isHost = false) : base(PacketType.Handshake)
    {
        Name = name;
        Id = id;
        IsHost = false;
    }
}