namespace Hitster.Networking;

public class Player
{
    public static List<Player> AllPlayers = new List<Player>();
    public static Player LocalPlayer;
    public static Player? CurrentPlayer { get; private set; }
    public static Dictionary<int, int> TokenGuesses { get; } = new Dictionary<int, int>();

    public static event Action? PlayerDataChanged;

    public static void SetCurrentPlayer(Player player)
    {
        CurrentPlayer = player;
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
    
    public Player(int id, string name, bool isHost)
    {
        Id = id;
        Name = name;
        IsHost = isHost;
        AllTracks = new List<TrackData>();
        Tokens = 2;
        
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
        if (newBalance < 0 || newBalance > 5)
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
                Form1.PlayTrack(track);
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

    public void RevealCurrentTrack()
    {
        if (CurrentTrack == null)
            return;
        
        if (CurrentTrackGuess != null && Program.CompareStrings(CurrentTrack.Name, CurrentTrackGuess.Item1) > 90
                                      && Program.CompareStrings(CurrentTrack.Artist, CurrentTrackGuess.Item2) > 90)
        {
            Task.Run(() => MessageBox.Show("Du hast den Song erraten und erhälst einen Token!", "Richtig!", MessageBoxButtons.OK,
                MessageBoxIcon.Information));
            NetworkManager.Instance.RpcAddToken(Id, 1);
        }
        Timeline.RevealTrack(this, CurrentTrack);
        CurrentTrack = null;
        CurrentTrackGuess = null;
    }

    public void GuessCurrentTrack(string title, string artist)
    {
        CurrentTrackGuess = new Tuple<string, string>(title, artist);
    }
}