namespace Hitster.Networking;

public class Player
{
    public static List<Player> Players = new List<Player>();
    public static Player LocalPlayer;
    public static Player? CurrentPlayer { get; private set; }

    public static event Action? PlayerDataChanged;

    public static void SetCurrentPlayer(Player player)
    {
        CurrentPlayer = player;
        PlayerDataChanged?.Invoke();
    }

    public static void AddPlayer(Player player)
    {
        Players.Add(player);
        Players.Sort((x, y) => x.Id.CompareTo(y.Id));
        PlayerDataChanged?.Invoke();
    }

    public static void RemovePlayer(Player player)
    {
        Players.Remove(player);
        PlayerDataChanged?.Invoke();
    }

    public int Id { get; }
    public string Name { get; }
    public bool IsHost { get; private set; }
    public List<TrackData> AllTracks { get; }
    public TrackData? CurrentTrack { get; private set; }
    
    public Player(int id, string name, bool isHost)
    {
        Id = id;
        Name = name;
        IsHost = isHost;
        AllTracks = new List<TrackData>();
        
        AddPlayer(this);
    }

    public void SetHost(bool isHost)
    {
        IsHost = isHost;
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

    public void ConfirmTrack()
    {
        CurrentTrack = null;
        Timeline.UpdateTimeline(this);
    }
}