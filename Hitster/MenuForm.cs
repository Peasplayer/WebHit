using Hitster.Networking;

namespace Hitster;

public partial class MenuForm : Form
{
    private TextBox nameBox;
    private TextBox ipBox;
    private Button startButton;
    private Button rulesButton;

    public MenuForm()
    {
        InitializeComponent();
        //Fenstergröße Einstellen
        Width = Screen.PrimaryScreen.Bounds.Width / 3;
        Height = Width * 9 / 16;

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
            ForeColor = Color.Black,
            Location = new Point(900 * ClientSize.Width / 1920, 150 * ClientSize.Height / 1080)
        };
        Controls.Add(nameLabel);

        // Textbox zur Eingabe des Namens
        nameBox = new TextBox
        {
            Size = new Size(500 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080),
            Location = new Point(900 * ClientSize.Width / 1920, 200 * ClientSize.Height / 1080),
            Font = new Font(Program.MontserratBold, 40F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel)
        };
        Controls.Add(nameBox);
        
        // Label für IP
        var ipLabel = new Label
        {
            Text = "Server",
            AutoSize = true,
            Font = new Font(Program.MontserratMediumItalic, 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            ForeColor = Color.Black,
            Location = new Point(900 * ClientSize.Width / 1920, 300 * ClientSize.Height / 1080)
        };
        Controls.Add(ipLabel);
        
        // Textbox zur Eingabe der IP
        ipBox = new TextBox
        {
            Size = new Size(500 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080),
            Location = new Point(900 * ClientSize.Width / 1920, 350 * ClientSize.Height / 1080),
            Font = new Font(Program.MontserratBold, 40F * ClientSize.Height / 1080, FontStyle.Regular, GraphicsUnit.Pixel),
            Text = "127.0.0.1"
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
        startButton.Location = new Point((ClientSize.Width - startButton.Width) / 2, 750 * ClientSize.Height / 1080);
        startButton.Click += StartButton_Click;
        Controls.Add(startButton);

        //Knopf um die Settings zu öffnen
        rulesButton = new Button
        {
            Size = new Size(200 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080),
            Text = "Regeln",
            Font = new Font(Program.MontserratMediumItalic, 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel),
            BackColor = Color.Gray,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        rulesButton.Location = new Point(ClientSize.Width - rulesButton.Width - 20,
            ClientSize.Height - rulesButton.Height - 20);
        rulesButton.Click += (_, _) => new SettingsForm().ShowDialog(this);
        Controls.Add(rulesButton);
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        if (nameBox.Text.Trim() == "" || ipBox.Text.Trim() == "")
        {
            MessageBox.Show("Bitte Name und IP eingeben!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Task.Run(() =>
        {
            try
            {
                NetworkManager.Connect("ws://" + ipBox.Text + ":9443", nameBox.Text);
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
                lobby.FormClosed += (_, _) => Show(); 
                lobby.Show();
                Hide();
            });
        });
    }
}