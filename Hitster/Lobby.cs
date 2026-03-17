using Hitster.Networking;

namespace Hitster;

public partial class Lobby : ResizeForm
{
    private static Lobby? _instance { get; set; }
    private static bool _gameStarting; // Ob sich die Lobby wegen Spielstart schließt

    public static void CloseForm(bool gameStarting = false)
    {
        _gameStarting = gameStarting;
        _instance?.Close();
    }

    //Wird aufgerufen wenn der Host das Spiel startet
    public static void OpenGameForm()
    {
        _instance?.BeginInvoke(() =>
        {
            var form = new Form1();
            form.Show();
            form.Size = _instance.Size;
            form.WindowState = _instance.WindowState;
            form.Location = _instance.Location;
            form.Invalidate();
            CloseForm(true);
        });
    }
    
    private Button StartButton { get; }
    private Button SettingsButton { get; }
    private List<PlayerCard> Cards { get; } //Liste aller Spieler-Karten
    
    public Lobby()
    {
        _instance = this;
        _gameStarting = false;
        FormClosing += (_, _) =>
        {
            Player.PlayerDataChanged -= _playerDataChanged;
            //Wenn das Spiel nicht startet wird die Verbindung zum Server getrennt und das Hauptmenü geöffnet
            if (!_gameStarting)
            {
                MenuForm.ShowForm();
            }
        };
        FormClosed += (_, _) => _instance = null;
        WindowState = FormWindowState.Maximized; //macht Vollbild
        ContentContainer.BackgroundImage = Image.FromStream(Program.GetResource("Lobby.png"));
        ContentContainer.BackgroundImageLayout = ImageLayout.Stretch;
        BackColor = Color.PaleTurquoise;
        InitializeComponent();
        
        //Start- und Einstellungs-Button erstellen
        StartButton = new Button
        {
            Cursor = Cursors.Hand,
            Text = "Start",
            Visible = false,
            Enabled = false
        };
        StartButton.Click += (_, _) => NetworkManager.RpcStart();
        Controls.Add(StartButton);
        RegisterResizeControl(StartButton, new SizeF(3, 1), new PointF(14.5f, 14), () =>
        {
            StartButton.Font = new Font(Program.MontserratBold, Math.Max(StartButton.Height * 0.8f, 1), FontStyle.Bold, GraphicsUnit.Pixel);
        });
        
        //Button um zu den Einstellungen zu kommen
        SettingsButton = new Button
        {
            Cursor = Cursors.Hand,
            Text = "Einstellungen",
            Visible = false 
        };
        SettingsButton.Click += (_, _) => new SettingsForm().ShowDialog(this);
        Controls.Add(SettingsButton);
        //Wird genau unter dem Start Button plazier egal wie groß das Forms ist
        RegisterResizeControl(SettingsButton, new SizeF(5, 1), new PointF(13.5f, 15.5f), () =>
        {
            SettingsButton.Font = new Font(Program.MontserratBold, Math.Max(SettingsButton.Height * 0.6f, 1), FontStyle.Bold, GraphicsUnit.Pixel);
        });
        
        Cards = new List<PlayerCard>(); //Liste für die Anzeige welche Spieler bereits in der Lobby sind

        //Generiert eine Anzeige aller Spieler die in der Lobby sind
        for (int i = 0; i < 6; i++)
        {
            var card = new PlayerCard();
            Cards.Add(card);
            Controls.Add(card);
            RegisterResizeControl(card, new SizeF(6, 3), new PointF(6 + i % 3 * 7, 4 + (i / 3) * 4), card.Render);
        }
        _refreshPlayers();

        Player.PlayerDataChanged += _playerDataChanged;
    }

    private static void _playerDataChanged()
    {
        _instance?.Invoke(_instance._refreshPlayers);
    }

    private void _refreshPlayers()
    {
        //Buttons sind nur für den Host sichbar
        StartButton.Visible = Player.LocalPlayer?.IsHost ?? false;
        StartButton.Enabled = Player.AllPlayers.Count >= 2; //Das Spiel kann nur gestartet werden wenn mindestens 2 Personen in der Lobby sind
        SettingsButton.Visible = Player.LocalPlayer?.IsHost ?? false;
        for (int i = 0; i < 6; i++)
        {
            var card = Cards[i];
            var player = i + 1 <= Player.AllPlayers.Count ? Player.AllPlayers[i] : null;
            card.SetPlayer(player);
        }
    }

    private class PlayerCard : Control
    {
        public Player? Player { get; private set; }
        
        private Label NameLabel { get; }

        //Erstellt die Anzeige wenn noch kein Spieler dort ist
        public PlayerCard()
        {
            BackColor = Color.PaleTurquoise;
            NameLabel = new Label();
            NameLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(NameLabel);
        }

        //Ein Spieler wird auf eine der Spieler Karten gesetzt
        public void SetPlayer(Player? player)
        {
            Player = player;
            if (Player != null)
            {
                NameLabel.Text = Player.Name;
                BackColor = Color.DarkTurquoise;
                NameLabel.Visible = true;
            }
            else
            {
                BackColor = Color.PaleTurquoise;
                NameLabel.Visible = false;
            }
            
            Render();
        }

        public void Render()
        {
            if (Player == null)
                return;

            NameLabel.Size = Size;
            NameLabel.Text = Player.Name + (Player.IsHost ? " [Host]" : ""); //Der Host wird markiert
            NameLabel.Font = new Font(Program.MontserratBold, Math.Max(Height * 0.2f, 1), Player.LocalPlayer == Player ? FontStyle.Underline : FontStyle.Regular, GraphicsUnit.Pixel);
        }
    }
}