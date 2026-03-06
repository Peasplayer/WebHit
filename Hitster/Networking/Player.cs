namespace Hitster.Networking;

public class Player
{
    public static List<Player> Players = new List<Player>();
    public static Player LocalPlayer;
    public static Player CurrentPlayer;

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
        
        Players.Add(this);
        if (id == 0)
            CurrentPlayer = this;
    }

    public void SetHost(bool isHost)
    {
        IsHost = isHost;
    }

    public void PlaceCurrentTrack(int index, TrackData? track = null)
    {
        if (track != null)
        {
            if (AllTracks.Count != 0)
            {
                CurrentTrack = track;
                Form1.Instance.Invoke(() => Form1.Instance.PlayTrack(track));
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