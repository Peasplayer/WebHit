using Hitster.Networking;
using NAudio.Wave;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    private static Form1? _instance;

    private Timeline OwnTimeline { get; }
    private Timeline OtherTimeline { get; }
    private FlowLayoutPanel PlayerArea { get; }
    private WasapiOut? _musicPlayer;
    private Button skipButton;
    private Button BuyTrackButton;
    
    private Label _timerLabel;
    private string _text;
    private int _timer;
    private int _maxTimer;
    private bool _timerIsRunning;

    public Form1()
    {
        _instance = this;
        FormClosing += (_, _) => Lobby.Instance?.Close();
        InitializeComponent();

        OwnTimeline = new Timeline();
        OwnTimeline.SetPlayer(Player.LocalPlayer);
        RegisterResizeControl(OwnTimeline, new SizeF(30, 4f), new PointF(1, 1), OwnTimeline.AfterResize);
        Controls.Add(OwnTimeline);

        OtherTimeline = new Timeline();
        OtherTimeline.SetPlayer(Player.AllPlayers.Find(p => p.Id != Player.LocalPlayer.Id));
        RegisterResizeControl(OtherTimeline, new SizeF(30, 4f), new PointF(1, 5.5f), OtherTimeline.AfterResize);
        Controls.Add(OtherTimeline);

        _timerLabel = new Label {
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Maroon,
            ForeColor = Color.White
        };
        RegisterResizeControl(_timerLabel, new SizeF(6, 1.5f), new PointF(25, 12), () => {
            _timerLabel.Font = new Font(Program.MontserratSemiBold, (int)(_timerLabel.Size.Height * 0.4f), GraphicsUnit.Pixel);
        });
        Controls.Add(_timerLabel);

        //Button zum Kaufen eines Liedes
        BuyTrackButton = new Button()
        {
            Text = $"Lied kaufen [{Settings.CurrentSettings.SongPrice}T]",
            Visible = true,
            Enabled = false,
            BackColor = Color.LightGreen,
            FlatStyle = FlatStyle.Flat
        };
        BuyTrackButton.Click += (_, _) => NetworkManager.RpcBuyTrack();
        RegisterResizeControl(BuyTrackButton, new SizeF(6f, 1.5f), new PointF(1f, 12f), () => 
        {
            BuyTrackButton.Font = new Font(Program.MontserratSemiBold, BuyTrackButton.Height * 0.3f, FontStyle.Bold, GraphicsUnit.Pixel);
        });
        Controls.Add(BuyTrackButton);

        //Button zum überspringen eines Liedes
        skipButton = new Button()
        {
            Text = "Lied überspringen [1T]",
            Visible = false,
            Enabled = false,
            BackColor = Color.Orange,
            FlatStyle = FlatStyle.Flat
        };
        skipButton.Click += (_, _) => NetworkManager.RpcSkipTrack();
        RegisterResizeControl(skipButton, new SizeF(6f, 1.5f), new PointF(7.2f, 12f), () => 
        {
            skipButton.Font = new Font(Program.MontserratSemiBold, skipButton.Height * 0.3f, FontStyle.Bold, GraphicsUnit.Pixel);
        });
        Controls.Add(skipButton);
        
        PlayerArea = new FlowLayoutPanel()
        {
            BackColor = Color.FromArgb(40, 40, 40),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true
        };
        RegisterResizeControl(PlayerArea, new SizeF(32, 4f), new PointF(0, 14), RenderPlayers);
        Controls.Add(PlayerArea);

        Player.PlayerDataChanged += () => Invoke(RenderPlayers);
    }

    public void _PlayTrack(TrackData track)
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
    
    private void RenderPlayers()
    {
        foreach (Control playerAreaControl in PlayerArea.Controls)
        {
            playerAreaControl.Dispose();
        }
        PlayerArea.Controls.Clear();

        BuyTrackButton.Enabled = Player.LocalPlayer?.Tokens >= 3;
        skipButton.Enabled = Player.LocalPlayer?.Tokens >= 1;
        skipButton.Visible = Player.CurrentPlayer?.Id == Player.LocalPlayer?.Id;

        foreach (var player in Player.AllPlayers)
        {
            var pad = new Padding((int) (PlayerArea.Width / 6f * 0.05f), (int) (PlayerArea.Height * 0.05f), 
                (int) (PlayerArea.Width / 6f * 0.05f), (int) (PlayerArea.Height * 0.05f));

            void Click(object? s, EventArgs a) {
                
                if (player != Player.LocalPlayer)
                    OtherTimeline.SetPlayer(player);
            };

            var playerCard = new Panel
            {
                Size = new Size(PlayerArea.Width / 6 - pad.Vertical, PlayerArea.Height - pad.Horizontal),
                BackColor = player.Id == Player.CurrentPlayer?.Id ? Color.Orange : Color.Gray,
                Margin = pad
            };
            playerCard.Click += Click;
            PlayerArea.Controls.Add(playerCard);

            var nameLabel = new Label
            {
                Text = player.Name,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Player.LocalPlayer == player ? Color.Black : Color.White,
                BackColor = Player.LocalPlayer == player ? Color.BurlyWood : Color.Black,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            nameLabel.Click += Click;
            playerCard.Controls.Add(nameLabel);
            
            var tokenLabel = new Label
            {
                Text = "Tokens: " + player.Tokens,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Location = new Point(0, nameLabel.Height),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            tokenLabel.Click += Click;
            playerCard.Controls.Add(tokenLabel);

            var trackLabel = new Label
            {
                Text = "Songs: " + player.AllTracks.Count(t => player.CurrentTrack != t),
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Location = new Point(0, nameLabel.Height * 2),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            trackLabel.Click += Click;
            playerCard.Controls.Add(trackLabel);
        }
    }

    private void _PlayerWon(Player player)
    {
        MessageBox.Show($"{(Player.LocalPlayer == player ? "Du hast" : player.Name + " hat")} gewonnen!", "Hitster Won", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static void PlayTrack(TrackData track)
    {
        _instance?.Invoke(() => _instance._PlayTrack(track));
    }

    public static void PlayerWon(Player player)
    {
        _instance?.Invoke(() => _instance._PlayerWon(player));
    }

    public static void SetOtherTimeline(Player player)
    {
        _instance?.Invoke(() => _instance.OtherTimeline.SetPlayer(player));
    }

    public static void StartTimer(string text, int time)
    {
        _instance?.Invoke(() =>
        {
            _instance._timer = 0;
            _instance._maxTimer = time;
            _instance._text = text;
            _instance._timerIsRunning = true;

            Task.Run(() => {
                Task.Delay(1000).Wait();
                while(_instance._timerIsRunning && _instance._text == text && _instance._timer <= _instance._maxTimer)
                {
                    _instance._timer++;
                    var remainingTime = _instance._maxTimer - _instance._timer;
                    _instance._timerLabel.Text = _instance._text + "\n" + (remainingTime / 60).ToString("00") + ":" +
                                                 (remainingTime % 60).ToString("00");
                    Task.Delay(1000).Wait();
                }
            });
        });
    }

    public static void StopTimer()
    {
        _instance?.Invoke(() =>
        {
            _instance._timerIsRunning = false;
            _instance._timerLabel.Text = "";
        });
    }

    public static void CloseForm()
    {
        _instance?.Close();
    }
}