namespace Hitster.Networking;

public class Settings
{
    public static Settings CurrentSettings { get; set; } 
        = new Settings(5, 2,10,5, 90, 3, 0);

    public static readonly string[] AllPacks = ["Standard", "Summer Party", "Schlager Party", "Guilty Pleasures", "Bayern1", 
        "Soundtracks", "Bingo-Pack", "Christmas", "Rock", "Celebration", "Platinum Edition", "100% US", "Hip Hop", "US-Pack"];

    public int MaxTokens { get; }
    public int StartTokens { get; }
    public int RequiredCards { get; }
    public int TokenPlaceTime { get; }
    public int GuessTime { get; }
    public int SongPrice { get; }
    public int Pack { get; }

    public Settings(int maxTokens, int startTokens, int requiredCards, int tokenPlaceTime, int guessTime, int songPrice, int pack)
    {
        MaxTokens = maxTokens;
        StartTokens = startTokens;
        RequiredCards = requiredCards;
        TokenPlaceTime = tokenPlaceTime;
        GuessTime = guessTime;
        SongPrice = songPrice;
        Pack = pack;
    }
}