using System.Net.Http.Headers;
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
        Size = new Size(400, 250);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(40, 40, 40);
        ForeColor = Color.White;

        //Label als Makierung damit der Nutzer weiß was er eingeben soll
        var titleLabel = new Label
        {
            Text = "Song Titel:",
            Location = new Point(20, 20),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 12, GraphicsUnit.Pixel)
        };
        Controls.Add(titleLabel);

        //TextBox zum eingeben des Liedtitels
        titleInput = new ComboBox()
        {
            Location = new Point(120, 20),
            Width = 230,
            Font = new Font(Program.MontserratSemiBold, 12, GraphicsUnit.Pixel),
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
                    $"https://api.deezer.com/search/track?q={titleInput.Text}&limit=5");
                var result = JsonConvert.DeserializeAnonymousType(str, 
                    new { data = new[] { new { title_short = "" } } });
                foreach (var r in result.data)
                {
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
            Text = "Interpret:",
            Location = new Point(20, 60),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 12, GraphicsUnit.Pixel)
        };
        Controls.Add(artistLabel);

        //Box zum auswählen des Interpreten
        artistInput = new ComboBox
        {
            Location = new Point(120, 60),
            Width = 230,
            Font = new Font(Program.MontserratSemiBold, 12, GraphicsUnit.Pixel),
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
            Text = "Überprüfen",
            Location = new Point(120, 100),
            Width = 230,
            Height = 30,
            BackColor = Color.Orange,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font(Program.MontserratBold, 12, FontStyle.Bold, GraphicsUnit.Pixel)
        };
        confirmButton.Click += CheckButton_Click;
        Controls.Add(confirmButton);

        //Lable das anzeigt ob die Eingabe richtig oder falsch war
        feedbackLabel = new Label
        {
            Location = new Point(20, 150),
            Width = 340,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Program.MontserratBold, 14, GraphicsUnit.Pixel)
        };
        Controls.Add(feedbackLabel);
    }

    //Wenn der Überorüfen Knopf gedrückt wird wird überprüft ob die Eingabe richtig ist 
    private void CheckButton_Click(object? sender, EventArgs e)
    {
        var currentTrack = Player.LocalPlayer?.CurrentTrack; //Speichert das aktuelle Lied
        
        //Überprüft ob ein Lied vorhanden ist
        if (currentTrack == null)
        {
            feedbackLabel.Text = "Kein Lied vorhanden";
            feedbackLabel.ForeColor = Color.Red;
            return;
        }

        var inputTitle = titleInput.Text.Trim(); //Speichert die Eingabe des Titels und entfernt leerzeichen
        var inputArtist = artistInput.Text.Trim(); //Speichert die Eingabe des Interpreten und entfernt leerzeichen

        bool titleCorrect = string.Equals(inputTitle, currentTrack.Name, StringComparison.OrdinalIgnoreCase); //Verglciht die Eingabe des Titels mit dem aktuellen Liedtitel und ignoriert Groß und kleinschreibung
        bool artistCorrect = string.Equals(inputArtist, currentTrack.Artist, StringComparison.OrdinalIgnoreCase); //Verglciht die Eingabe des Interpretens mit dem aktuellen Interpreten und ignoriert Groß und kleinschreibung

        //Gibt aus wenn die Eingabe richtig ist
        if (titleCorrect && artistCorrect)
        {
            feedbackLabel.Text = "Richtig! Du erhältst einen Token";
            feedbackLabel.ForeColor = Color.LightGreen;
        }
        else
        {
            feedbackLabel.Text = "Titel oder Interpret flasch";
        }
    }
}