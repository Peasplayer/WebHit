namespace Hitster;

public class TrackData
{
    public string Id { get; }
    public string Name { get; }
    public string Artist { get; }
    public string Link { get; }
    public int ReleaseYear { get; }
    
    public TrackData(string id, string name, string artist, string link, int releaseYear)
    {
        Id = id;
        Name = name;
        Artist = artist;
        Link = link;
        ReleaseYear = releaseYear;
    }
}