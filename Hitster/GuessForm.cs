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
        var currentTrack = Player.LocalPlayer?.CurrentTrack;
        
        /*var inputTitle = RemoveSpecialCaracters(titleInput.Text);
        var inputArtist = RemoveSpecialCaracters(artistInput.Text);

        var realTitle = RemoveSpecialCaracters(currentTrack.Name);
        var realArtist = RemoveSpecialCaracters(currentTrack.Artist);*/

        var titleMatch = CompareStrings(currentTrack.Name, titleInput.Text);
        var artistMatch = CompareStrings(currentTrack.Artist, artistInput.Text);

        if (titleMatch > 90 && artistMatch > 90)
        {
            feedbackLabel.Text = "Richtig! Du erhältst einen Token";
            feedbackLabel.ForeColor = Color.LightGreen;
        }
        else
        {
            feedbackLabel.Text = "Falsch";
            feedbackLabel.ForeColor = Color.Red;
        }
    }

    //Entfernt Sonderueichen aus den Titeln der Lieder
    private string RemoveSpecialCaracters(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        text = text.ToLower();
        text = text.Replace(" ", "");
        text = text.Replace("/", "");
        text = text.Replace("-", "");

        return text;
    }
    
    // Quelle: https://medium.com/@tarakshah/this-article-explains-how-to-check-the-similarity-between-two-string-in-percentage-or-score-from-0-83e206bf6bf5
    private static double CompareStrings(string str1, string str2)
    {
        var pairs1 = WordLetterPairs(str1.ToUpper());
        var pairs2 = WordLetterPairs(str2.ToUpper());

        int intersection = 0;
        int union = pairs1.Count + pairs2.Count;

        for (int i = 0; i < pairs1.Count; i++)
        {
            for (int j = 0; j < pairs2.Count; j++)
            {
                if (pairs1[i] == pairs2[j])
                {
                    intersection++;
                    pairs2.RemoveAt(j);//Must remove the match to prevent "AAAA" from appearing to match "AA" with 100% success
                    break;
                }
            }
        }

        return (2.0 * intersection * 100) / union; //returns in percentage
        //return (2.0 * intersection) / union; //returns in score from 0 to 1
    }
    
    private static List<string> WordLetterPairs(string str)
    {
        var AllPairs = new List<string>();

        // Tokenize the string and put the tokens/words into an array
        string[] Words = Regex.Split(str, @"\s");

        // For each word
        for (int w = 0; w < Words.Length; w++)
        {
            if (!string.IsNullOrEmpty(Words[w]))
            {
                // Find the pairs of characters
                String[] PairsInWord = LetterPairs(Words[w]);

                for (int p = 0; p < PairsInWord.Length; p++)
                {
                    AllPairs.Add(PairsInWord[p]);
                }
            }
        }
        return AllPairs;
    }

    // Generates an array containing every two consecutive letters in the input string
    private static string[] LetterPairs(string str)
    {
        int numPairs = str.Length - 1;
        string[] pairs = new string[numPairs];

        for (int i = 0; i < numPairs; i++)
        {
            pairs[i] = str.Substring(i, 2);
        }
        return pairs;
    }
}