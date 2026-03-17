namespace HitsterServer.MusicData;
//Alle Daten eines Liedes
public struct TrackData
{
    public string Id { get; }
    public string Name { get; }
    public string Artist { get; }
    public string Link { get; }
    public int ReleaseYear;

    public TrackData(string id, string name, string artist, string link)
    {
        Id = id;
        Name = name;
        Artist = artist;
        Link = link;
    }
}