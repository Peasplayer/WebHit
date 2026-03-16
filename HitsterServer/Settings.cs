namespace HitsterServer;

public class Settings
{
    public static Settings CurrentSettings;
    
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