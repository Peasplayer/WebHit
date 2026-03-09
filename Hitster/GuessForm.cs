using Hitster.Networking;

namespace Hitster;

public partial class GuessForm : Form
{
    private TextBox titleInput; //Textbos zum Eingeben des liedtitels
    private ComboBox artistInput; //Box zum Auswählen des Interpreten
    private Button confirmButton; //Knopf zum bestätigen der Eingabe
    private Label feedbackLabel; //Anzeige des Ergebnisses

    public static List<string> AllArtists = new(); //Liste für alle Interpreten

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

        LoadArtists();

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
        titleInput = new TextBox
        {
            Location = new Point(120, 20),
            Width = 230,
            Font = new Font(Program.MontserratSemiBold, 12, GraphicsUnit.Pixel)
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
            AutoCompleteMode = AutoCompleteMode.SuggestAppend, //Damit der Nutzer den Interpreten richtig schreibt damit keine überprüfung nötig ist
            AutoCompleteSource = AutoCompleteSource.CustomSource
        };
        Controls.Add(artistInput);

        //Fügt alle Interpreten in die ComboBox als auswahl ein
        var artistsArray = AllArtists.ToArray(); //Wandelt die Liste der Interpreten in ein Array um damit es in der ComboBox angezeigt werden kann
        var autoComplete = new AutoCompleteStringCollection();
        autoComplete.AddRange(artistsArray);

        artistInput.AutoCompleteCustomSource = autoComplete;
        artistInput.Items.AddRange(artistsArray);

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

    //Speichert für alle Spieler die Interpreten der gelegten Songs in eine Liste
    private void LoadArtists()
    {
        AllArtists.Clear();

        foreach (var player in Player.Players)
        {
            foreach (var track in player.AllTracks)
            {
                if (!AllArtists.Contains(track.Artist))
                {
                    AllArtists.Add(track.Artist);
                }
            }

            if (player.CurrentTrack != null && !AllArtists.Contains(player.CurrentTrack.Artist))
            {
                AllArtists.Add(player.CurrentTrack.Artist);
            }
        }
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