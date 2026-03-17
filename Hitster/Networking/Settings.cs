namespace Hitster.Networking;

public class Settings
{
    //Speichert die ausgewählten Einstellungen und hat bereits Standard-Einstellungen gespeichert
    public static Settings CurrentSettings { get; set; } 
        = new Settings(5, 2,10,5, 90, 3, 0);

    //Liste mit allen Packs von den man wählen kann
    public static readonly string[] AllPacks = ["Standard", "Summer Party", "Schlager Party", "Guilty Pleasures", "Bayern1", 
        "Soundtracks", "Bingo-Pack", "Christmas", "Rock", "Celebration", "Platinum Edition", "100% US", "Hip Hop", "US-Pack"];

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