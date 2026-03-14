using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Hitster.Networking;
using Newtonsoft.Json;

namespace Hitster;

public partial class GuessForm : Form
{
    private ComboBox titleInput; //Box zum Auswähle des Liedtitels
    private ComboBox artistInput; //Box zum Auswählen des Interpreten
    private Button confirmButton; //Knopf zum bestätigen der Eingabe
    private Label feedbackLabel; //Anzeige des Ergebnisses

    public GuessForm()
    {
        InitializeComponent();

        //Algemeine einstellungen des Forms
        Text = "Song erraten";
        Size = new Size(590, 300);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(40, 40, 40);
        ForeColor = Color.White;

        //Label als Makierung damit der Nutzer weiß was er eingeben soll
        var titleLabel = new Label
        {
            Text = "Titel",
            Location = new Point(20, 20),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(titleLabel);

        //TextBox zum eingeben des Liedtitels
        titleInput = new ComboBox()
        {
            Location = new Point(200, 20),
            Width = 350,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel),
            Tag = null
        };
        titleInput.KeyDown += async (sender, e) => //Verhindert das der Nutzer etwas eingeben kann was nicht in der Liste ist
        {
            if (e.KeyData == Keys.Enter && titleInput.Tag == null)
            {
                titleInput.Tag = "locked"; //Verhindert das die Eingabe mehrmals bestätigt wird
                Console.WriteLine(titleInput.Text);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WebHit-Test", "0.0.1"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var str = await client.GetStringAsync(
                    $"https://api.deezer.com/search/track?q={titleInput.Text}&limit=10");
                var result = JsonConvert.DeserializeAnonymousType(str, 
                    new { data = new[] { new { title_short = "" } } });
                foreach (var r in result.data)
                {
                    if (!titleInput.Items.Contains(r.title_short))
                        titleInput.Items.Add(r.title_short);
                }
                titleInput.ResetText();
                titleInput.DropDownStyle = ComboBoxStyle.DropDownList;
                titleInput.SelectedIndex = 0;
            }
        };
        Controls.Add(titleInput);
    
        //Label als Makierung damit der Nutzer weiß was er eingeben soll
        var artistLabel = new Label
        {
            Text = "Interpret",
            Location = new Point(20, 80),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(artistLabel);

        //Box zum auswählen des Interpreten
        artistInput = new ComboBox
        {
            Location = new Point(200, 80),
            Width = 350,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel),
            Tag = null
        };
        artistInput.KeyDown += async (sender, e) => //Verhindert das der Nutzer etwas eingeben kann was nicht in der Liste ist
        {
            if (e.KeyData == Keys.Enter && artistInput.Tag == null)
            {
                artistInput.Tag = "locked"; //Verhindert das die Eingabe mehrmals bestätigt wird
                Console.WriteLine(artistInput.Text);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WebHit-Test", "0.0.1"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var str = await client.GetStringAsync(
                    $"https://api.deezer.com/search/artist?q={artistInput.Text}&limit=5");
                var result = JsonConvert.DeserializeAnonymousType(str, 
                    new { data = new[] { new { name = "" } } });
                foreach (var r in result.data)
                {
                    if (!artistInput.Items.Contains(r.name))
                        artistInput.Items.Add(r.name);
                }
                artistInput.ResetText();
                artistInput.DropDownStyle = ComboBoxStyle.DropDownList;
                artistInput.SelectedIndex = 0;
            }
        };
        Controls.Add(artistInput);

        //Button zum bestätigen der Eingabe
        confirmButton = new Button
        {
            Text = "Einloggen",
            Location = new Point(350, 150),
            Width = 200,
            Height = 80,
            BackColor = Color.Orange,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font(Program.MontserratBold, 24, FontStyle.Bold, GraphicsUnit.Pixel)
        };
        confirmButton.Click += (_, _) =>
        {
            Player.LocalPlayer.GuessCurrentTrack(titleInput.Text, artistInput.Text);
            Close();
        };
        Controls.Add(confirmButton);
    }
}