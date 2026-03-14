using Hitster.Networking;
using Hitster.Networking.Packets;

namespace Hitster;

public partial class MenueForm : Form
{
    private TextBox nameBox;
    private TextBox ipBox;
    private Button startButton;
    private Button settingsButton;

    public MenueForm()
    {
        InitializeComponent();
        //Fenstergröße Einstellen
        Height = Screen.PrimaryScreen.Bounds.Height / 2;
        Width = Height * 16 / 9;

        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        CreateUI();
    }

    private void CreateUI()
    {
        //Textbox zur eingabe des Namens
        nameBox = new TextBox();
        nameBox.Size = new Size(500 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080);
        nameBox.Location = new Point(900 * ClientSize.Width / 1920, 200 * ClientSize.Height / 1080);
        nameBox.Font = new Font("Segoe UI", 40F * ClientSize.Height / 1080, FontStyle.Regular, GraphicsUnit.Pixel);
        Controls.Add(nameBox);
        
        //Label für Name
        var nameLabel = new Label();
        nameLabel.Text = "Name:";
        nameLabel.AutoSize = true;
        nameLabel.Font = new Font("Segoe UI", 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel);
        nameLabel.ForeColor = Color.Black;
        nameLabel.Location = new Point(900 * ClientSize.Width / 1920, 150 * ClientSize.Height / 1080);
        Controls.Add(nameLabel);


        //Textbox zur eingabe der IP
        ipBox = new TextBox();
        ipBox.Size = new Size(500 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080);
        ipBox.Location = new Point(900 * ClientSize.Width / 1920, 350 * ClientSize.Height / 1080);
        ipBox.Font = new Font("Segoe UI", 40F * ClientSize.Height / 1080, FontStyle.Regular, GraphicsUnit.Pixel);
        ipBox.Text = "127.0.0.1";
        Controls.Add(ipBox);
        
        //Label für IP
        var ipLabel = new Label();
        ipLabel.Text = "Server IP:";
        ipLabel.AutoSize = true;
        ipLabel.Font = new Font("Segoe UI", 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel);
        ipLabel.ForeColor = Color.Black;
        ipLabel.Location = new Point(900 * ClientSize.Width / 1920, 300 * ClientSize.Height / 1080);
        Controls.Add(ipLabel);

        //Knopf zum verbinden mit dem Server
        startButton = new Button();
        startButton.Size = new Size(500 * ClientSize.Width / 1920, 120 * ClientSize.Height / 1080);
        startButton.Location = new Point((ClientSize.Width - startButton.Width) / 2, 750 * ClientSize.Height / 1080);
        startButton.Text = "Start";
        startButton.Font = new Font("Segoe UI", 40F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel);
        startButton.BackColor = Color.Orange;
        startButton.FlatStyle = FlatStyle.Flat;
        startButton.Cursor = Cursors.Hand;

        startButton.Click += StartButton_Click;

        Controls.Add(startButton);

        //Knopf um die Settings zu öffnen
        settingsButton = new Button();
        settingsButton.Size = new Size(200 * ClientSize.Width / 1920, 80 * ClientSize.Height / 1080);
        settingsButton.Location = new Point(ClientSize.Width - settingsButton.Width - 20,
            ClientSize.Height - settingsButton.Height - 20);
        settingsButton.Text = "Settings";
        settingsButton.Font = new Font("Segoe UI", 30F * ClientSize.Height / 1080, FontStyle.Bold, GraphicsUnit.Pixel);
        settingsButton.BackColor = Color.Gray;
        settingsButton.FlatStyle = FlatStyle.Flat;
        settingsButton.Cursor = Cursors.Hand;

        settingsButton.Click += (_, _) => { };

        Controls.Add(settingsButton);
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
                new NetworkManager("ws://" + ipBox.Text + ":8443", nameBox.Text);
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show($"Server nicht erreichbar!\n{ex.Message}", "Verbindungsfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
                return;
            }

            BeginInvoke(new Action(() =>
            {
                Lobby lobby = new Lobby();
                lobby.Location = this.Location;
                lobby.FormClosed += (s, args) => this.Close(); 
                lobby.Show();
                this.Hide();
            }));
        });
    }
}