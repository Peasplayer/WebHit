namespace HitsterServer;

public class Settings
{
    //Speichert die aktuellen Einstellungen
    public static Settings CurrentSettings;
    
    public int MaxTokens { get; } //Wie viele Tokens ein Spieler maximal haben darf

    public int StartTokens { get; } //Mit wie vielen Tokens jeder Spieler das Spiel beginnt

    public int RequiredCards { get; } //Wie viele korrekt einsortierte Karten ein Spieler braucht um zu gewinnen

    public int TokenPlaceTime { get; } //Wie viele Sekunden die anderen Spieler haben um einen Token zu platzieren

    public int GuessTime { get; } //Wie viele Sekunden der aktuelle Speiler zum Raten Zeit hat

    public int SongPrice { get; } //Wie viele Tokens es kostet ein Lied zu kaufen

    public int Pack { get; } //Das ausgewählte Pack

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