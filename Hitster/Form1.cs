using Hitster.Networking;
using NAudio.Wave;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    public static Form1? Instance { get; private set; }

    private Timeline OwnTimeline { get; }
    private Timeline OtherTimeline { get; }
    private readonly Button _confirmButton;

    private WasapiOut? _musicPlayer;
    
    public Form1()
    {
        Instance = this;
        FormClosing += (_, _) => Instance = null;
        InitializeComponent();
        
        OwnTimeline = new Timeline();
        OwnTimeline.SetPlayer(Player.LocalPlayer);
        RegisterResizeControl(OwnTimeline, new SizeF(30, 3.5f), new PointF(1, 1), OwnTimeline.AfterResize);
        Controls.Add(OwnTimeline);

        OtherTimeline = new Timeline();
        OtherTimeline.SetPlayer(Player.Players.Find(p => p.Id != Player.LocalPlayer.Id));
        RegisterResizeControl(OtherTimeline, new SizeF(30, 3.5f), new PointF(1, 10), OtherTimeline.AfterResize);
        Controls.Add(OtherTimeline);
        
        _confirmButton = new Button
        {
            BackgroundImageLayout = ImageLayout.Zoom,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            BackgroundImage = Image.FromStream(Program.GetResource("Gruener Harken.png"))
        };
        _confirmButton.Click += (_, _) =>
        {
            if (_musicPlayer != null)
                _musicPlayer.Stop();
            
            NetworkManager.Instance.RpcConfirmTrack();
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
        Task.Run(() =>
        {
            while (_musicPlayer.PlaybackState == PlaybackState.Playing && Player.CurrentPlayer.CurrentTrack != null) ;
            _musicPlayer.Stop();
        });
    }
}