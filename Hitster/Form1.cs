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

    public Form1()
    {
        _instance = this;
        FormClosing += (_, _) => Lobby.Instance?.Close();
        InitializeComponent();

        OwnTimeline = new Timeline();
        OwnTimeline.SetPlayer(Player.LocalPlayer);
        RegisterResizeControl(OwnTimeline, new SizeF(30, 3.5f), new PointF(1, 1), OwnTimeline.AfterResize);
        Controls.Add(OwnTimeline);

        OtherTimeline = new Timeline();
        OtherTimeline.SetPlayer(Player.AllPlayers.Find(p => p.Id != Player.LocalPlayer.Id));
        RegisterResizeControl(OtherTimeline, new SizeF(30, 3.5f), new PointF(1, 10), OtherTimeline.AfterResize);
        Controls.Add(OtherTimeline);

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

        //Button zum überspringen eines Liedes
        skipButton = new Button()
        {
            Text = "Lied überspringen (1 Token)",
            Visible = false,
            BackColor = Color.Orange,
            FlatStyle = FlatStyle.Flat
        };
        skipButton.Click += (_, _) =>
        {
            if (Player.LocalPlayer.Tokens >= 1)
                NetworkManager.RpcSkipTrack();
            else
                MessageBox.Show("Du hast nicht genug Tokens!", "Achtung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        };
        RegisterResizeControl(skipButton, new SizeF(6f, 1.5f), new PointF(1f, 6.5f), () => 
        {
            skipButton.Font = new Font(Program.MontserratSemiBold, skipButton.Height * 0.3f, FontStyle.Bold, GraphicsUnit.Pixel);
        });
        Controls.Add(skipButton);
        //Button zum Kaufen eines Liedes
        BuyTrackButton = new Button()
        {
            Text = "Lied kaufen (3 Tokens)",
            Visible = true,
            BackColor = Color.LightGreen,
            FlatStyle = FlatStyle.Flat
        };
        BuyTrackButton.Click += (_, _) =>
        {
            if (Player.LocalPlayer.Tokens >= 3)
                NetworkManager.RpcBuyTrack();
            else
                MessageBox.Show("Du hast nicht genug Tokens! Du brauchst 3.", "Achtung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        };
        RegisterResizeControl(BuyTrackButton, new SizeF(6f, 1.5f), new PointF(1f, 8.2f), () => 
        {
            BuyTrackButton.Font = new Font(Program.MontserratSemiBold, BuyTrackButton.Height * 0.3f, FontStyle.Bold, GraphicsUnit.Pixel);
        });
        Controls.Add(BuyTrackButton);
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

        foreach (var player in Player.AllPlayers)
        {
            var pad = new Padding((int) (PlayerArea.Width / 6f * 0.05f), (int) (PlayerArea.Height * 0.05f), 
                (int) (PlayerArea.Width / 6f * 0.05f), (int) (PlayerArea.Height * 0.05f));
            var playerCard = new Panel
            {
                Size = new Size(PlayerArea.Width / 6 - pad.Vertical, PlayerArea.Height - pad.Horizontal),
                BackColor = player.Id == Player.CurrentPlayer?.Id ? Color.Orange : Color.Gray,
                Margin = pad
            };
            playerCard.Click += (_, _) =>
            {
                if (player != Player.LocalPlayer)
                    OtherTimeline.SetPlayer(player);
            };
            PlayerArea.Controls.Add(playerCard);

            var nameLabel = new Label
            {
                Text = player.Name,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Player.LocalPlayer == player ? Color.BurlyWood : Color.White,
                BackColor = Color.Black,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            nameLabel.Click += (_, _) =>
            {
                if (player != Player.LocalPlayer)
                    OtherTimeline.SetPlayer(player);
            };
            playerCard.Controls.Add(nameLabel);
            
            var tokenLabel = new Label
            {
                Text = "Tokens: " + player.Tokens,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.2f)),
                Location = new Point(0, nameLabel.Height),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            tokenLabel.Click += (_, _) =>
            {
                if (player != Player.LocalPlayer)
                    OtherTimeline.SetPlayer(player);
            };
            playerCard.Controls.Add(tokenLabel);
        }

        if (skipButton != null)
        {
            if(Player.CurrentPlayer?.Id == Player.LocalPlayer.Id)
            {
                skipButton.Visible = true;
            }
            else
            {
                skipButton.Visible = false;
            }
        }
    }

    private void _PlayerWon(Player player)
    {
        MessageBox.Show($"{(Player.LocalPlayer == player ? "Du hast " : player.Name + " hat")} gewonnen!", "Hitster Won", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
}