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
        this.Height = 600;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;

        //Titel der Regeln
        titleLabel = new Label()
        {
            Text = "Spielregeln",
            Font = new Font("Arial", 28, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(330, 30)
        };
        Controls.Add(titleLabel);
        
        //Label für die Kapitelüberschrift
        contentTitleLabel = new Label()
        {
            Font = new Font("Arial", 18, FontStyle.Bold),
            AutoSize = false,
            Width = 700,
            Height = 40,
            Location = new Point(100, 120)
        };
        Controls.Add(contentTitleLabel);

        //Label für die Regeln
        contentLabel = new Label()
        {
            Font = new Font("Arial", 12),
            AutoSize = false,
            Width = 700,
            Height = 250,
            Location = new Point(100, 180)
        };
        Controls.Add(contentLabel);

        //Button zum Navigieren durch die Regeln
        backButton = new Button()
        {
            Text = "Zurück",
            Width = 120,
            Height = 40,
            Location = new Point(200, 470)
        };
        backButton.Click += BackButton_Click;
        Controls.Add(backButton);

        //Button zum Navigieren durch die Regeln
        nextButton = new Button()
        {
            Text = "Weiter",
            Width = 120,
            Height = 40,
            Location = new Point(580, 470)
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
         switch (chapter)
    {
        case 0:
            contentTitleLabel.Text = "Einleitung (1/10)";
            contentLabel.Text = "Hitster ist ein Musik Partyspiel. Ihr hört bekannte Songs aus der Musikgeschichte.\n\nEure Aufgabe ist es die Songs in der richtigen zeitlichen Reihenfolge auf eurer persönlichen Zeitleiste anzuordnen.";
            break;

        case 1:
            contentTitleLabel.Text = "Spielziel (2/10)";
            contentLabel.Text = "Versuche möglichst viele Songs korrekt in deine Zeitachse einzuordnen.\n\nWer als erster 10 lieder in die richtige Reinfolge bringen konnte geweinnt das Spiel.";
            break;

        case 2:
            contentTitleLabel.Text = "Der Zug eines Spielers (3/10)";
            contentLabel.Text = "Es wird eine neue Karte gezogen und das dazu passende Lied automatisch abgespielt.\n\nJetzt kann der Spieler entscheiden wo er die Karte in seiner Zeitachse einordnen möchte.";
            break;

        case 3:
            contentTitleLabel.Text = "Karte platzieren (5/10)";
            contentLabel.Text = "Um eine Karte zu plazieren nutze die weißen vierecke über den schon auf der Zeitachse liegenden Karten.\n\nZum bestätigen das du die Karte dort ablegen möchtest drücke einfach auf sie.";
            break;

        case 4:
            contentTitleLabel.Text = "Auflösung (6/10)";
            contentLabel.Text = "Wenn die Stelle bestätigt wurde wird die Karte umgedreht.\n\n Wenn sie richtig plaziert wurde bleibt sie an der stelle liegen.\n" +
                                "Ist sie allerdings Falsch plaziert wird sie Rot und verschwindet nach kurzer Zeit.\n"+
                                "Nun ist der nächste Spieler an der Reihe";
            break;

        case 5:
            contentTitleLabel.Text = "Titel und Künstler nennen (7/10)";
            contentLabel.Text = "Wenn du denkst das du den Titel und den Interpreten des liedes kennst kannst du diese durchs cklicken des ... Buttons machen.\n\n Errätst du richtig erhältst du einen Hitster Token.\n" +
                                "Denk aber dran das man nur 5 Hitster Token gleichzeitig haben kann.";
            break;

        case 6:
            contentTitleLabel.Text = "Hitster-Token (8/10)";
            contentLabel.Text = "Token können für besondere Aktionen genutzt werden:\n\n" +
                                "• Für 1 Token kannst du ein lied überspringen und ein neues erhalten\n" +
                                "• Wenn du denkst das ein Gegner ein Lied falsch eingeordnet hat kannst du 1 Token auf seine Zeitachse legen. Hattest du recht erhältst du die Karte deines Gegners. Lagst du falsch verlierst du deinen Token. Wetten mehrer erhält die Person die am nähsten zum Erscheinungsjahr ware die Karte\n" +
                                "• Du kannst drei Token gegen eine Karte austauschen die du direkt, ohne Raten, auf deine Zeitachse legen darfst. Mit dieser Karte darfst du allerdings nicht gewinnen. Sie darf also nicht deine 10 sein.";
            break; ;

        case 7:
            contentTitleLabel.Text = "Spielende (10/10)";
            contentLabel.Text = "Das Spiel endet sobald ein Spieler 10 Karten korrekt in seiner Zeitachse liegen hat.\n\nDieser Spieler gewinnt das Spiel und wird zum Hitster gekrönt!";
            break;
        }
    }
}