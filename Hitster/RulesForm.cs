namespace Hitster;

public partial class RulesForm : Form
{
    private int currentRuleChapter = 0; //Um zu wissen welche Regeln angezeigt werden

    private Label titleLabel;
    private Label contentTitleLabel; 
    private Label contentLabel;

    private Button nextButton;
    private Button backButton;
    public RulesForm()
    {
        InitializeComponent();
        SetupUI();
        ShowChapter(currentRuleChapter);
    }

    private void SetupUI()
    {
        //Allgemeine Einstellungen des Forms
        this.Width = 900;
        this.Height = 650;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;

        //Titel der Regeln
        titleLabel = new Label()
        {
            Text = "Spielregeln",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Program.MontserratBold, 28, FontStyle.Bold),
            AutoSize = true,
            MinimumSize = new Size(900, 0)
        };
        Controls.Add(titleLabel);
        
        //Label für die Kapitelüberschrift
        contentTitleLabel = new Label()
        {
            Font = new Font(Program.MontserratMediumItalic, 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(100, 120)
        };
        Controls.Add(contentTitleLabel);

        //Label für die Regeln
        contentLabel = new Label()
        {
            Font = new Font(Program.MontserratSemiBold, 12),
            AutoSize = false,
            Width = 700,
            Height = 300,
            Location = new Point(100, 180)
        };
        Controls.Add(contentLabel);

        //Button zum Navigieren durch die Regeln
        backButton = new Button()
        {
            Text = "Zurück",
            Width = 120,
            Height = 40,
            Location = new Point(200, 520)
        };
        backButton.Click += BackButton_Click;
        Controls.Add(backButton);

        //Button zum Navigieren durch die Regeln
        nextButton = new Button()
        {
            Text = "Weiter",
            Width = 120,
            Height = 40,
            Location = new Point(580, 520)
        };
        nextButton.Click += NextButton_Click;
        Controls.Add(nextButton);
    }

    //Regel "Kapitel" wird runtergezählt da auf den zurück Button geklickt wurde
    private void BackButton_Click(object sender, EventArgs e)
    {
        if (currentRuleChapter > 0)
        {
            currentRuleChapter--;
            ShowChapter(currentRuleChapter);
        }
    }

    //Regel "Kapitel" wird hochgezählt da auf den weiter Button geklickt wurde
    private void NextButton_Click(object sender, EventArgs e)
    {
        if (currentRuleChapter < 7)
        {
            currentRuleChapter++;
            ShowChapter(currentRuleChapter);
        }
    }

    //Switch case um die Regeln je nach Kapitel anzuzeigen
    private void ShowChapter(int chapter)
    {
        backButton.Visible = chapter != 0;
        nextButton.Visible = chapter != 7;
        
         switch (chapter)
        {
            case 0:
                contentTitleLabel.Text = "Einleitung (1/8)";
                contentLabel.Text = "Hitster ist ein Musik Partyspiel. Ihr hört bekannte Songs aus der Musikgeschichte. Eure Aufgabe ist es die Songs in der richtigen zeitlichen Reihenfolge auf eurer persönlichen Zeitleiste anzuordnen.";
                break;

            case 1:
                contentTitleLabel.Text = "Spielziel (2/8)";
                contentLabel.Text = "Versuche möglichst viele Songs korrekt in deine Zeitachse einzuordnen. Wer als erster eine bestimmte Anzahl an Liedern in die richtige Reihenfolge bringen konnte gewinnt das Spiel.";
                break;

            case 2:
                contentTitleLabel.Text = "Der Zug eines Spielers (3/8)";
                contentLabel.Text = "Es wird eine neue Karte gezogen und das dazu passende Lied automatisch abgespielt. Jetzt kann der Spieler entscheiden wo er die Karte in seiner Zeitachse einordnen möchte.";
                break;

            case 3:
                contentTitleLabel.Text = "Karte platzieren (4/8)";
                contentLabel.Text = "Um eine Karte zu platzieren nutze die weißen vierecke über den schon auf der Zeitachse liegenden Karten. Zum bestätigen, dass du die Karte dort ablegen möchtest, klicke sie an.";
                break;

            case 4:
                contentTitleLabel.Text = "Auflösung (5/8)";
                contentLabel.Text = "Wenn die Stelle bestätigt wurde wird die Karte umgedreht. Wenn sie richtig platziert wurde bleibt sie an der stelle liegen. " +
                                    "Ist sie allerdings Falsch platziert wird sie Rot und verschwindet nach kurzer Zeit. "+
                                    "Nun ist der nächste Spieler an der Reihe";
                break;

            case 5:
                contentTitleLabel.Text = "Titel und Künstler nennen (6/8)";
                contentLabel.Text = "Wenn du denkst, dass du den Titel und den Interpreten des Liedes kennst, kannst du diese beim Bestätigen erraten. Rätst du richtig erhältst du einen Hitster Token. " +
                                    "Du kannst nur eine gewisse Anzahl von Token sammeln.";
                break;

            case 6:
                contentTitleLabel.Text = "Hitster-Token (7/8)";
                contentLabel.Text = "• Für 1 Token kannst du ein Lied überspringen und ein anderes erraten\n" +
                                    "• Wenn du denkst, dass ein Gegner ein Lied falsch eingeordnet hat, kannst du einen Token auf seine Zeitachse legen. Hattest du Recht, erhältst du die Karte deines Gegners. Lagst du falsch verlierst du deinen Token.\n" +
                                    "• Du kannst drei Token gegen eine Karte austauschen die du direkt, ohne Raten, bekommst.";
                break; ;

            case 7:
                contentTitleLabel.Text = "Spielende (8/8)";
                contentLabel.Text = "Das Spiel endet sobald ein Spieler eine bestimmte Anzahl an Karten korrekt in seiner Zeitachse liegen hat.\n\nDieser Spieler gewinnt das Spiel und wird zum Hitster gekrönt!";
                break;
        }
    }
}