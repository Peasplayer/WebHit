namespace Hitster.Networking;

public class Player
{
    public static List<Player> AllPlayers = new List<Player>();
    public static Player? LocalPlayer { get; private set; }
    public static Player? CurrentPlayer { get; private set; }
    public static Dictionary<int, int> TokenGuesses { get; } = new Dictionary<int, int>();

    public static event Action? PlayerDataChanged;

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
        AllPlayers.Sort((x, y) => x.Id.CompareTo(y.Id));
        PlayerDataChanged?.Invoke();
    }

    public static void RemovePlayer(Player player)
    {
        AllPlayers.Remove(player);
        PlayerDataChanged?.Invoke();
    }

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
    public List<TrackData> AllTracks { get; }
    public TrackData? CurrentTrack { get; private set; }
    public Tuple<string, string>? CurrentTrackGuess { get; private set; }
    public int Tokens { get; private set; }
    private bool _isGuessing;
    
    public Player(int id, string name, bool isHost)
    {
        Id = id;
        Name = name;
        IsHost = isHost;
        AllTracks = new List<TrackData>();
        Tokens = Math.Min(Settings.CurrentSettings.StartTokens, Settings.CurrentSettings.MaxTokens);
        
        AddPlayer(this);
    }

    public void SetHost(bool isHost)
    {
        IsHost = isHost;
        PlayerDataChanged?.Invoke();
    }

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
            if (AllTracks.Count != 0)
            {
                CurrentTrack = track;
                _isGuessing = true;
                Form1.PlayTrack(track);

                Task.Run(() =>
                {
                    var timeout = 0;
                    while (_isGuessing && timeout <= Settings.CurrentSettings.GuessTime)
                    {
                        timeout++;
                        Task.Delay(1000).Wait();
                    }
                    if (_isGuessing)
                        ConfirmTrack();
                });
            }
            AllTracks.Add(track);
        }
        else if (CurrentTrack != null)
        {
            AllTracks.Remove(CurrentTrack);
            AllTracks.Insert(index, CurrentTrack);
        }

        Timeline.UpdateTimeline(this);
    }

    public void AddTrack(TrackData track)
    {
        AllTracks.Add(track);
        AllTracks.Sort((t1, t2) => t1.ReleaseYear.CompareTo(t2.ReleaseYear));
        Timeline.UpdateTimeline(this);
    }

    public void LooseTrack(TrackData track)
    {
        AllTracks.Remove(track);
        Timeline.UpdateTimeline(this);
    }

    public void ConfirmTrack()
    {
        _isGuessing = false;
        NetworkManager.RpcConfirmTrack();
}

    public void RevealCurrentTrack()
    {
        if (CurrentTrack == null)
            return;
        
        if (CurrentTrackGuess != null && Program.CompareStrings(CurrentTrack.Name, CurrentTrackGuess.Item1) > 90
                                      && Program.CompareStrings(CurrentTrack.Artist, CurrentTrackGuess.Item2) > 90)
        {
            Task.Run(() => MessageBox.Show("Du hast den Song erraten und erhälst einen Token!", "Richtig!", MessageBoxButtons.OK,
                MessageBoxIcon.Information));
            NetworkManager.RpcAddToken(Id, 1);
        }
        Timeline.RevealTrack(this, CurrentTrack);
        CurrentTrack = null;
        CurrentTrackGuess = null;
    }
    //methode zum entfernen einer Karte
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

    public void GuessCurrentTrack(string title, string artist)
    {
        CurrentTrackGuess = new Tuple<string, string>(title, artist);
    }
}