using Hitster.Networking;
using NAudio.Wave;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    public static Form1 Instance;

    public Timeline Timeline { get; }
    private Card? currentCard;
    private readonly Button _confirmButton;

    private WasapiOut? _musicPlayer;
    
    public Form1()
    {
        Instance = this;
        InitializeComponent();
        
        Timeline = new Timeline();
        Timeline.SlotClicked += OnSlotClicked;
        Timeline.SetPlayer(Player.LocalPlayer);
        RegisterResizeControl(Timeline, new SizeF(30, 3.5f), new PointF(1, 1), Timeline.AfterResize);
        Controls.Add(Timeline);
        
        _confirmButton = new Button
        {
            BackgroundImageLayout = ImageLayout.Zoom,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            Cursor =  Cursors.Hand,
        };
        
        _confirmButton.BackgroundImage = Image.FromStream(Program.GetResource("Gruener Harken.png"));

        _confirmButton.Click += (_, _) =>
        {
            if (_musicPlayer != null)
                _musicPlayer.Stop();
            
            Timeline.Player.ConfirmTrack();
            Timeline.ToggleSlots(false);
        };
        Controls.Add(_confirmButton);
        RegisterResizeControl(_confirmButton, new Size(2, 2), new Point(1, 7));
    }

    public void PlayTrack(TrackData track)
    {
        if (_musicPlayer != null)
            _musicPlayer.Stop();

        using var mf = new MediaFoundationReader(track.Link);
        _musicPlayer = new WasapiOut();
        _musicPlayer.Init(mf);
        _musicPlayer.Play();
    }

    private void OnSlotClicked(int index)
    {
        if (currentCard == null) return;

        Player.CurrentPlayer.PlaceCurrentTrack(index);
    }
}