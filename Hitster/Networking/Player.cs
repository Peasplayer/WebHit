namespace Hitster.Networking;

public class Player
{
    public static List<Player> Players = new List<Player>();
    public static Player LocalPlayer;
    public static Player CurrentPlayer;

    public int Id { get; }
    public string Name { get; }
    public List<TrackData> AllTracks { get; }
    public TrackData? CurrentTrack { get; private set; }
    
    public Player(int id, string name)
    {
        Id = id;
        Name = name;
        AllTracks = new List<TrackData>();
        
        Players.Add(this);
        if (id == 0)
            CurrentPlayer = this;
    }

    public void PlaceCurrentTrack(int index, TrackData? track = null)
    {
        Console.WriteLine("Track move");
        if (track != null)
        {
            if (AllTracks.Count != 0)
            {
                Form1.Instance.Invoke(() => Form1.Instance.PlayTrack(track));
                CurrentTrack = track;
                if (Id == LocalPlayer.Id)
                    Form1.Instance.Invoke(() => Form1.Instance.Timeline.ToggleSlots(true));
            }
            AllTracks.Add(track);
        }
        else if (CurrentTrack != null)
        {
            AllTracks.Remove(CurrentTrack);
            AllTracks.Insert(index, CurrentTrack);
        }
        
        Form1.Instance.Invoke(() => Form1.Instance.Timeline.UpdateTracks());
    }

    public void RpcConfirmTrack()
    {
        // Do Network Stuff
    }

    public void ConfirmTrack()
    {
        CurrentTrack = null;
        Form1.Instance.Invoke(() => Form1.Instance.Timeline.UpdateTracks());
    }
}