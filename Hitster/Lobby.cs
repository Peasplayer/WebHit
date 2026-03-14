using Hitster.Networking;

namespace Hitster;

public partial class Lobby : ResizeForm
{
    public static Lobby? Instance { get; private set; }
    
    private Button StartButton { get; }
    private Button RulesButton { get; }
    private List<PlayerCard> Cards { get; }
    
    public Lobby()
    {
        Instance = this;
        FormClosing += (_, _) => Instance = null;
        WindowState = FormWindowState.Maximized; //macht Vollbild
        InitializeComponent();
        
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
        
        //Button um zu den Regeln zu kommen
        RulesButton = new Button
        {
            Cursor = Cursors.Hand,
            Text = "Regeln",
            Visible = true 
        };
        RulesButton.Click += (_, _) => new RulesForm().ShowDialog(this);
        Controls.Add(RulesButton);
        //Wird genau unter dem Start Button plazier egal wie groß das Forms ist
        RegisterResizeControl(RulesButton, new SizeF(3, 1), new PointF(14.5f, 15.5f), () =>
        {
            RulesButton.Font = new Font(Program.MontserratBold, Math.Max(RulesButton.Height * 0.6f, 1), FontStyle.Bold, GraphicsUnit.Pixel);
        });
        
        Cards = new List<PlayerCard>();

        for (int i = 0; i < 6; i++)
        {
            var card = new PlayerCard();
            Cards.Add(card);
            Controls.Add(card);
            RegisterResizeControl(card, new SizeF(6, 3), new PointF(6 + i % 3 * 7, 4 + (i / 3) * 4), card.Render);
        }
        _refreshPlayers();
        
        Player.PlayerDataChanged += () => Invoke(Instance._refreshPlayers);
    }

    private void _refreshPlayers()
    {
        StartButton.Visible = Player.LocalPlayer?.IsHost ?? false;
        StartButton.Enabled = Player.AllPlayers.Count >= 2;
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

        public PlayerCard()
        {
            BackColor = Color.BurlyWood;
            NameLabel = new Label();
            NameLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(NameLabel);
        }

        public void SetPlayer(Player? player)
        {
            Player = player;
            if (Player != null)
            {
                NameLabel.Text = Player.Name;
                BackColor = Player.LocalPlayer == Player ? Color.RosyBrown : Color.Brown;
                NameLabel.Visible = true;
            }
            else
            {
                BackColor = Color.BurlyWood;
                NameLabel.Visible = false;
            }
            
            Render();
        }

        public void Render()
        {
            if (Player == null)
                return;

            NameLabel.Size = Size;
            NameLabel.Text = Player.Name + (Player.IsHost ? " [Host]" : "");
            NameLabel.Font = new Font(Program.MontserratBold, Math.Max(Height * 0.2f, 1), FontStyle.Bold, GraphicsUnit.Pixel);
        }
    }
}