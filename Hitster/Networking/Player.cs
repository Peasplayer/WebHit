namespace Hitster.Networking;

public class Player
{
    public static List<Player> AllPlayers = new List<Player>(); //Liste mit allen Spielern
    public static Player? LocalPlayer { get; private set; } //Der lokale Spieler
    public static Player? CurrentPlayer { get; private set; } //Der Spieler der momentan am zug ist
    public static Dictionary<int, int> TokenGuesses { get; } = new Dictionary<int, int>(); //Speichert welcher Spieler wo seinen Token abgelegt hat

    public static event Action? PlayerDataChanged; //Event für wenn sich Daten der Spieler änden

    // Löscht alle Spieler-Daten
    public static void Reset()
    {
        AllPlayers.Clear();
        LocalPlayer = null;
        CurrentPlayer = null;
        TokenGuesses.Clear();
    }

    public static void SetCurrentPlayer(Player player)
    {
        CurrentPlayer = player;
        PlayerDataChanged?.Invoke();
    }

    public static void SetLocalPlayer(Player player)
    {
        LocalPlayer = player;
        PlayerDataChanged?.Invoke();
    }

    public static void AddPlayer(Player player)
    {
        AllPlayers.Add(player);
        AllPlayers.Sort((x, y) => x.Id.CompareTo(y.Id)); //Liste nach ID sotieren damit die Reihnfolge immer gleich ist
        PlayerDataChanged?.Invoke();
    }

    public static void RemovePlayer(Player player)
    {
        AllPlayers.Remove(player);
        PlayerDataChanged?.Invoke();
    }

    //Sucht einen Spieler anhand seiner ID. Wenn dieser nicht exestiert gibt es einen Fehler
    public static Player GetPlayer(int id)
    {
        var p = AllPlayers.Find(p => p.Id == id);
        if (p == null)
            throw new InvalidOperationException($"Player with id {id} not found");
        return p;
    }

    public int Id { get; }
    public string Name { get; }
    public bool IsHost { get; private set; }
    public List<TrackData> AllTracks { get; } //Alle Songs die der Spieler in seiner Timeline hat, auch nicht umgedeckte
    public TrackData? CurrentTrack { get; private set; } //Das Lied das der Spieler momentan erraten muss
    public Tuple<string, string>? CurrentTrackGuess { get; private set; } //Der Tipp des Spieler für das erraten des Interpreten und Titels
    public int Tokens { get; private set; }
    private bool _isGuessing; //Variable um zu wissen ob der Timer gerade läuft
    
    public Player(int id, string name, bool isHost)
    {
        Id = id;
        Name = name;
        IsHost = isHost;
        AllTracks = new List<TrackData>();
        Tokens = Math.Min(Settings.CurrentSettings.StartTokens, Settings.CurrentSettings.MaxTokens); //Ein Spieler kann niemals mehr Tokens am anfang bekommen als die maximale Anzahl
        
        AddPlayer(this); //Der neue Spieler wird in die Liste hinzugefügt
    }

    public void SetHost(bool isHost)
    {
        IsHost = isHost;
        PlayerDataChanged?.Invoke();
    }
    
    //Tokens hinzufügen oder, wenn ein Negativer Wert übergeben wird, abziehen
    public void AddTokens(int tokens)
    {
        var newBalance = Tokens + tokens;
        if (newBalance < 0 || newBalance > Settings.CurrentSettings.MaxTokens)
            return;
        Tokens = newBalance;
        PlayerDataChanged?.Invoke();
    }

    public void PlaceCurrentTrack(int index, TrackData? track = null)
    {
        if (track != null)
        {
            //Wenn bereits ein Lied auf der Timeline liegt muss der Spieler raten
            if (AllTracks.Count != 0)
            {
                CurrentTrack = track;
                _isGuessing = true; //Timer wird aktiviert
                Form1.PlayTrack(track); //Musik wird abgespielt
                
                //Timer wird gestartet
                Task.Run(() =>
                {
                    var timeout = 0;
                    //Timer läuft bis der Spieler nicht mehr rät oder die Zeit abgelaufen ist
                    while (_isGuessing && timeout <= Settings.CurrentSettings.GuessTime)
                    {
                        Task.Delay(1000).Wait();
                        timeout++;
                    }
                    //Wenn die Zeit abgelaufen ist ohne das geraten wurde wird das Lied umgedreht
                    if (_isGuessing)
                        ConfirmTrack();
                });
                Form1.StartTimer("Raten", Settings.CurrentSettings.GuessTime); //Startet den visuellen Timer
            }
            AllTracks.Add(track); //Das Lied der Timeline hinzufügen
        }
        //Spieler verschiebt ein vorhandenes Lied
        else if (CurrentTrack != null)
        {
            AllTracks.Remove(CurrentTrack); //Altes Lied wird entfernt
            AllTracks.Insert(index, CurrentTrack); //Das Lied wird an die neue Position gelegt
        }

        Timeline.UpdateTimeline(this); //Timeline neu zeichnen
    }

    //Fügt ein Lied der Timeline hinzu und sortiert die Timline nach Erscheinungsjahr
    public void AddTrack(TrackData track)
    {
        AllTracks.Add(track);
        AllTracks.Sort((t1, t2) => t1.ReleaseYear.CompareTo(t2.ReleaseYear));
        Timeline.UpdateTimeline(this);
    }

    //Entfernt ein Lied aus der Timeline
    public void LooseTrack(TrackData track)
    {
        AllTracks.Remove(track);
        Timeline.UpdateTimeline(this);
    }

    //Wird ausgeführt wenn ein Spieler das Lied bestätigt
    public void ConfirmTrack()
    {
        if (LocalPlayer != this)
            return;

        _isGuessing = false; //Stoppt den Timer
        NetworkManager.RpcConfirmTrack();
    }
    //Löst den Song auf und Prüft ob Tokens verdient wurden
    public void RevealCurrentTrack()
    {
        if (CurrentTrack == null)
            return;
        
        //Wenn der Spieler Interpret und Titel geraten hat wird überprüft ob dieses zu mindestens 90% dem richtigen Angaben entspricht
        if (CurrentTrackGuess != null && Program.CompareStrings(CurrentTrack.Name, CurrentTrackGuess.Item1) > 90
                                      && Program.CompareStrings(CurrentTrack.Artist, CurrentTrackGuess.Item2) > 90)
        {
            //Wenn es richtig ist wird ein Pop up angezeigt
            Task.Run(() => MessageBox.Show("Du hast den Song erraten und erhälst einen Token!", "Richtig!", MessageBoxButtons.OK,
                MessageBoxIcon.Information));
            NetworkManager.RpcAddToken(Id, 1); //Spieler erhält ein Token
        }
        //Karte wird aufgedeckt
        Timeline.RevealTrack(this, CurrentTrack);
        CurrentTrack = null;
        CurrentTrackGuess = null;
    }
    //Methode zum entfernen einer Karte
    public void DiscardCurrentTrack()
    {
        if (CurrentTrack != null)
        {
            AllTracks.Remove(CurrentTrack);
            CurrentTrack = null;
            CurrentTrackGuess = null;
            Timeline.UpdateTimeline(this); 
        }
    }

    //Speichert den eingegebenen Interpret und Titel
    public void GuessCurrentTrack(string title, string artist)
    {
        CurrentTrackGuess = new Tuple<string, string>(title, artist);
    }
}