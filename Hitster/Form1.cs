using Hitster.Networking;
using NAudio.Wave;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    private static Form1? _instance; //Speichert die aktuelle Instanze dieser Form damit man auf sie zugreifen kann

    private Timeline OwnTimeline { get; } 
    private Timeline OtherTimeline { get; }
    private FlowLayoutPanel PlayerArea { get; } //Bereich in dem alle Spieler angezeigt werden
    private WasapiOut? _musicPlayer; //Musikwiedergabe
    //Knöpfe
    private Button skipButton;
    private Button buyTrackButton;
    
    //Timer
    private Label _timerLabel;
    private string _timerText;
    private int _timer;
    private int _maxTimer;
    private bool _timerIsRunning;

    public Form1()
    {
        _instance = this;
        ContentContainer.BackColor = BackColor = Color.BlanchedAlmond;
        FormClosing += (_, _) =>
        {
            Player.PlayerDataChanged -= _playerDataChanged; // Alte Events entfernen
            Timeline.Reset();
            _musicPlayer?.Stop();
            MenuForm.ShowForm();
        };
        FormClosed += (_, _) => _instance = null;
        InitializeComponent();

        //Die eigene Timeline erstellen und oben anzeigen
        OwnTimeline = new Timeline();
        OwnTimeline.SetPlayer(Player.LocalPlayer);
        RegisterResizeControl(OwnTimeline, new SizeF(30, 4f), new PointF(1, 1), OwnTimeline.Render);
        Controls.Add(OwnTimeline);

        //Die Timeline eines gegners erstellen und unten anzeigen lassen
        OtherTimeline = new Timeline();
        OtherTimeline.SetPlayer(Player.AllPlayers.Find(p => p.Id != Player.LocalPlayer.Id));
        RegisterResizeControl(OtherTimeline, new SizeF(30, 4f), new PointF(1, 5.5f), OtherTimeline.Render);
        Controls.Add(OtherTimeline);

        //Timer anzeigen
        _timerLabel = new Label {
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Maroon,
            ForeColor = Color.White
        };
        RegisterResizeControl(_timerLabel, new SizeF(6, 1.5f), new PointF(24, 12), () => {
            _timerLabel.Font = new Font(Program.MontserratSemiBold, (int)(_timerLabel.Size.Height * 0.4f), GraphicsUnit.Pixel);
        });
        Controls.Add(_timerLabel);

        //Button zum Kaufen eines Liedes
        buyTrackButton = new Button()
        {
            Text = $"Lied kaufen [{Settings.CurrentSettings.SongPrice}T]",
            Visible = true,
            Enabled = false,
            BackColor = Color.LightGreen,
            FlatStyle = FlatStyle.Flat
        };
        buyTrackButton.Click += (_, _) => NetworkManager.RpcBuyTrack();
        RegisterResizeControl(buyTrackButton, new SizeF(6f, 1.5f), new PointF(2f, 12f), () => 
        {
            buyTrackButton.Font = new Font(Program.MontserratSemiBold, buyTrackButton.Height * 0.3f, FontStyle.Bold, GraphicsUnit.Pixel);
        });
        Controls.Add(buyTrackButton);

        //Button zum überspringen eines Liedes
        skipButton = new Button()
        {
            Text = "Lied überspringen [1T]",
            Visible = false,
            Enabled = false,
            BackColor = Color.LightGreen,
            FlatStyle = FlatStyle.Flat
        };
        skipButton.Click += (_, _) => NetworkManager.RpcSkipTrack();
        RegisterResizeControl(skipButton, new SizeF(6f, 1.5f), new PointF(8.2f, 12f), () => 
        {
            skipButton.Font = new Font(Program.MontserratSemiBold, skipButton.Height * 0.3f, FontStyle.Bold, GraphicsUnit.Pixel);
        });
        Controls.Add(skipButton);
        
        //Spielerübersichet als Flowlayout Panel zum automatisch nebeneinander sotieren
        PlayerArea = new FlowLayoutPanel()
        {
            BackColor = Color.DarkSlateGray,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false, //Verbietet Zeilensprünge
            AutoScroll = true //Verbietet das Scrollen
        };
        RegisterResizeControl(PlayerArea, new SizeF(32, 4f), new PointF(0, 14), RenderPlayers);
        Controls.Add(PlayerArea);

        Player.PlayerDataChanged += _playerDataChanged; //Wenn sich die daten eines Spielers ändern wird die Spieler anzeige neu geladen
    }

    private static void _playerDataChanged()
    {
        _instance?.Invoke(_instance.RenderPlayers);
    }

    public void _PlayTrack(TrackData track)
    {
        //Musik stoppen fals noch Musik läuft
        if (_musicPlayer != null)
            _musicPlayer.Stop();

        //Lied über einen Link abspielen
        using var mf = new MediaFoundationReader(track.Link);
        _musicPlayer = new WasapiOut();
        _musicPlayer.Init(mf);
        _musicPlayer.Play();
        Task.Run(() =>
        {
            //Sobald der Spieler geraten hat oder keine Musik mehr läuft wird die Musik beendet
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

        //Buttons nur klickbar bzw. anzeigen wenn ein Spieler ihn auch nutzen kann
        buyTrackButton.Enabled = Player.LocalPlayer?.Tokens >= 3;
        skipButton.Enabled = Player.LocalPlayer?.Tokens >= 1 && !Timeline.AllowTokenPlacement;
        skipButton.Visible = Player.CurrentPlayer?.Id == Player.LocalPlayer?.Id;

        //Für jeden Spieler eine Profil-Karte zu Beginn des Spieles zeichnen
        foreach (var player in Player.AllPlayers)
        {
            var pad = new Padding((int) (PlayerArea.Width / 6f * 0.05f), (int) (PlayerArea.Height * 0.05f), 
                (int) (PlayerArea.Width / 6f * 0.05f), (int) (PlayerArea.Height * 0.05f));

            //Wenn man auf einen Spieler in der Spielerübersicht drückt wird dessen Timeline angezeigt
            void Click(object? s, EventArgs a) {
                
                if (player != Player.LocalPlayer)
                    OtherTimeline.SetPlayer(player);
            };

            //Panel für die Spielerinformationen
            var playerCard = new Panel
            {
                Size = new Size(PlayerArea.Width / 6 - pad.Vertical, PlayerArea.Height - pad.Horizontal),
                BackColor = Color.LightSlateGray,
                Margin = pad
            };
            playerCard.Click += Click;
            PlayerArea.Controls.Add(playerCard);

            //Name
            var nameLabel = new Label
            {
                Text = player.Name + (player == Player.LocalPlayer ? " [Du]" : ""),
                TextAlign = ContentAlignment.MiddleCenter,
                //Der aktuelle Spieler hat eine helle Farbe und die anderen eine dunkle
                ForeColor = player.Id == Player.CurrentPlayer?.Id ? Color.Black : Color.White,
                BackColor = player.Id == Player.CurrentPlayer?.Id ? Color.BurlyWood : Color.Black,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            nameLabel.Click += Click;
            playerCard.Controls.Add(nameLabel);
            
            //Tokens Anzeige
            var tokenLabel = new Label
            {
                Text = "".PadLeft(player.Tokens, 'Ⓣ'),
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Location = new Point(0, nameLabel.Height),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            tokenLabel.Click += Click;
            playerCard.Controls.Add(tokenLabel);

            //Anzahl der songs die eine Person hat
            var trackLabel = new Label
            {
                Text = "".PadLeft(player.AllTracks.Count(t => player.CurrentTrack != t), '♪'),
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(playerCard.Width, (int)(playerCard.Height * 0.3f)),
                Location = new Point(0, nameLabel.Height * 2),
                Font = new Font(Program.MontserratSemiBold, playerCard.Height * 0.1f, FontStyle.Bold, GraphicsUnit.Pixel)
            };
            trackLabel.Click += Click;
            playerCard.Controls.Add(trackLabel);
        }
    }

    //Statitsche Methoden um Funktionen der Form von außerhalb nutzen zu können
    
    public static void PlayTrack(TrackData track)
    {
        _instance?.Invoke(() => _instance._PlayTrack(track));
    }

    public static void SetOtherTimeline(Player player)
    {
        _instance?.Invoke(() => _instance.OtherTimeline.SetPlayer(player));
    }

    // Logik des Timers
    public static void StartTimer(string text, int time)
    {
        _instance?.Invoke(() =>
        {
            _instance._timer = 0;
            _instance._maxTimer = time;
            _instance._timerText = text;
            _instance._timerIsRunning = true;

            Task.Run(() => {
                Task.Delay(1000).Wait();
                while(_instance._timerIsRunning && _instance._timerText == text && _instance._timer <= _instance._maxTimer)
                {
                    _instance._timer++;
                    var remainingTime = _instance._maxTimer - _instance._timer;
                    _instance._timerLabel.Text = _instance._timerText + "\n" + (remainingTime / 60).ToString("00") + ":" +
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