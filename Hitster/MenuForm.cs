using Hitster.Networking;

namespace Hitster;

public partial class MenuForm : Form
{
    private static MenuForm? _instance;
    private TextBox nameBox;
    private TextBox ipBox;
    private Button startButton;
    private Button rulesButton;

    public MenuForm()
    {
        _instance = this;

        InitializeComponent();
        //Fenstergröße Einstellen
        Width = Screen.PrimaryScreen.Bounds.Width / 3;
        Height = Width * 9 / 16;
        BackgroundImage = Image.FromStream(Program.GetResource("Menu.png"));
        BackgroundImageLayout = ImageLayout.Stretch;

        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        CreateUI();
    }

    private void CreateUI()
    {
        // Label für Name
        var nameLabel = new Label
        {
            Text = "Name",
            AutoSize = true,
            Font = new Font(Program.MontserratMediumItalic, 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(384 * ClientSize.Width / 1920, 150 * ClientSize.Height / 1080)
        };
        Controls.Add(nameLabel);

        // Textbox zur Eingabe des Namens
        nameBox = new TextBox
        {
            Size = new Size(500 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080),
            Location = new Point(384 * ClientSize.Width / 1920, 200 * ClientSize.Height / 1080),
            Font = new Font(Program.MontserratBold, 40F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            BackColor = Color.LightGray
        };
        nameBox.KeyDown += (_, e) =>
        {
            if (e.KeyData == Keys.Enter)
                StartButton_Click(null, e);
        };
        Controls.Add(nameBox);
        
        // Label für IP
        var ipLabel = new Label
        {
            Text = "Server",
            AutoSize = true,
            Font = new Font(Program.MontserratMediumItalic, 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(1036 * ClientSize.Width / 1920, 150 * ClientSize.Height / 1080)
        };
        Controls.Add(ipLabel);
        
        // Textbox zur Eingabe der IP
        ipBox = new TextBox
        {
            Size = new Size(500 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080),
            Location = new Point(1036 * ClientSize.Width / 1920, 200 * ClientSize.Height / 1080),
            Font = new Font(Program.MontserratBold, 40F * ClientSize.Height / 1080, FontStyle.Regular, GraphicsUnit.Pixel),
            Text = "127.0.0.1",
            BackColor = Color.LightGray
        };
        Controls.Add(ipBox);

        // Knopf zum Verbinden mit dem Server
        startButton = new Button
        {
            Size = new Size(500 * ClientSize.Width / 1920, 120 * ClientSize.Height / 1080),
            Text = "Spiel beitreten",
            Font = new Font(Program.MontserratBold, 40F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            BackColor = Color.Orange,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        startButton.Location = new Point((ClientSize.Width - startButton.Width) / 2, 800 * ClientSize.Height / 1080);
        startButton.Click += StartButton_Click;
        Controls.Add(startButton);

        //Knopf um die Settings zu öffnen
        rulesButton = new Button
        {
            Size = new Size(200 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080),
            Text = "Regeln",
            Font = new Font(Program.MontserratMediumItalic, 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            BackColor = Color.LightGray,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        rulesButton.Location = new Point(ClientSize.Width - rulesButton.Width - 20,
            ClientSize.Height - rulesButton.Height - 20);
        rulesButton.Click += (_, _) => new RulesForm().ShowDialog(this);
        Controls.Add(rulesButton);
    }

    private void StartButton_Click(object? sender, EventArgs e)
    {
        if (nameBox.Text.Trim() == "" || ipBox.Text.Trim() == "") //Fals es keinen Input gibt 
        {
            MessageBox.Show("Bitte Name und IP eingeben!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);  //Warnung fals IP oder Name leer ist
            return;
        }

        Task.Run(() =>
        {
            try
            {
                NetworkManager.Connect("ws://" + ipBox.Text + ":9443", nameBox.Text); //Kombiniert die eingabe der IP mir dem Port zum verbinden mit dem Server und übergibt den eingegebenen Namen
            }
            catch (Exception ex)
            {
                Invoke(() => MessageBox.Show($"Server nicht erreichbar!\n{ex.Message}", 
                    "Verbindungsfehler", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return;
            }

            Invoke(() =>
            {
                var lobby = new Lobby();
                lobby.Location = Location;
                lobby.Show();
                Hide();
            });
        });
    }

    public static void ShowForm()
    {
        NetworkManager.Disconnect();
        _instance?.Show();
    }
}