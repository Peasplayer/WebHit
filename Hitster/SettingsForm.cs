using Hitster.Networking;

namespace Hitster;

public partial class SettingsForm : Form
{
    public SettingsForm()
    {
        InitializeComponent();
        
        // Algemeine Einstellungen des Forms
        Text = "Einstellungen";
        BackColor = Color.FromArgb(40, 40, 40);
        ForeColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(800, 640);

        // Eingebfelder und Labels für die einzelnen Einstellungen
        var maxTokenLabel = new Label
        {
            Text = "Maximale Anzahl an Tokens", 
            Location = new Point(20, 20), 
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var maxTokensBox = new NumericUpDown {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Value = Settings.CurrentSettings.MaxTokens,
            DecimalPlaces = 0,
            Location = new Point(400, 20),
            Width = 100,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(maxTokenLabel);
        Controls.Add(maxTokensBox);
        
        var startTokeLabel = new Label
        {
            Text = "Anfangs Tokens",
            Location = new Point(20, 80),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var startTokensBox = new NumericUpDown {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Value = Settings.CurrentSettings.StartTokens,
            DecimalPlaces = 0,
            Location = new Point(400, 80),
            Width = 100,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(startTokeLabel);
        Controls.Add(startTokensBox);
        
        var requiredCardsLabel = new Label
        {
            Text = "Benötigte Karten für Sieg",
            Location = new Point(20, 140),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var requiredCardsBox = new NumericUpDown {
            Minimum = 2,
            Maximum = 10,
            Increment = 1,
            Value = Settings.CurrentSettings.RequiredCards,
            DecimalPlaces = 0,
            Location = new Point(400, 140),
            Width = 100,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(requiredCardsLabel);
        Controls.Add(requiredCardsBox);
        
        var tokenPlaceTime = new Label
        {
            Text = "Zeit zum Token-Platzieren",
            Location = new Point(20, 200),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var tokenPlaceTimeBox = new NumericUpDown {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Value = Settings.CurrentSettings.TokenPlaceTime,
            DecimalPlaces = 0,
            Location = new Point(400, 200),
            Width = 100,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(tokenPlaceTime);
        Controls.Add(tokenPlaceTimeBox);
        
        var guessTimeLabel = new Label
        {
            Text = "Rate-Zeit (Sek.)",
            Location = new Point(20, 260),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var guessTimeBox = new NumericUpDown {
            Minimum = 5,
            Maximum = 100,
            Increment = 5,
            Value = Settings.CurrentSettings.GuessTime,
            DecimalPlaces = 0,
            Location = new Point(400, 260),
            Width = 100,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(guessTimeLabel);
        Controls.Add(guessTimeBox);
        
        var songPriceLabel = new Label
        {
            Text = "Preis für Lied-Kauf",
            Location = new Point(20, 320),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var songPriceBox = new NumericUpDown {
            Minimum = 1,
            Maximum = 100,
            Increment = 1,
            Value = Settings.CurrentSettings.SongPrice,
            DecimalPlaces = 0,
            Location = new Point(400, 320),
            Width = 100,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        Controls.Add(songPriceLabel);
        Controls.Add(songPriceBox);
        
        var packLabel = new Label
        {
            Text = "Song-Pack",
            Location = new Point(20, 380),
            AutoSize = true,
            Font = new Font(Program.MontserratSemiBold, 24, GraphicsUnit.Pixel)
        };
        var packBox = new ComboBox {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(400, 380),
            Width = 250,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        foreach (var p in Settings.AllPacks)
            packBox.Items.Add(p);
        packBox.SelectedIndex = Settings.CurrentSettings.Pack;
        Controls.Add(packLabel);
        Controls.Add(packBox);

        var saveButton = new Button
        {
            Text = "Speichern",
            Location = new Point(600, 500),
            Size = new Size(150, 60),
            ForeColor = Color.Black,
            BackColor = Color.LightGreen, 
            FlatStyle = FlatStyle.Flat,
            Font = new Font(Program.MontserratBold, 24, GraphicsUnit.Pixel)
        };
        saveButton.Click += (_, _) =>
        {
            Settings.CurrentSettings = new Settings((int)maxTokensBox.Value, (int)startTokensBox.Value,
                (int)requiredCardsBox.Value, (int)tokenPlaceTimeBox.Value, (int)guessTimeBox.Value,
                (int)songPriceBox.Value, packBox.SelectedIndex);
            Close();
        };
        Controls.Add(saveButton);
    }
}