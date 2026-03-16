using System.Drawing.Drawing2D;
using Hitster.Networking;

namespace Hitster;

public sealed class Card : Panel
{
    public bool IsRevealed { get; private set; }
    public bool IsCorrect { get; private set; }
    public TrackData Track { get; }

    private Label _artist;
    private Label _year;
    private Label _title;

    public Card(TrackData track)
    {
        Track = track;

        BackColor = Color.Black;
        BackgroundImage = Image.FromStream(Program.GetResource("Karte.jpg"));
        BackgroundImageLayout = ImageLayout.Zoom;
        BorderStyle = BorderStyle.FixedSingle;

        _artist = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = Track.Artist.Replace("&", "&&"), Visible = false };
        _year = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = Track.ReleaseYear.ToString(), Visible = false };
        _title = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = Track.Name.Replace("&", "&&"), Visible = false };
        Controls.AddRange(_artist, _year, _title);

        void ResizeLabels()
        {
            if (Height == 0 || Width == 0)
                return;

            var smallSize = new Size((int)(Width * 0.9), (int)(Height * 0.35));
            var smallFontSize = (int)(smallSize.Height * 0.7 / 3);
            if (smallFontSize == 0)
                return;
            
            _artist.Location = new Point((int)(Width * 0.05), 0);
            _title.Location = new Point((int)(Width * 0.05), (int)(Height * 0.65));
            _artist.Size = _title.Size = smallSize;
            _artist.Font = new Font(Program.MontserratSemiBold, smallFontSize, GraphicsUnit.Pixel);
            _title.Font = new Font(Program.MontserratMediumItalic, smallFontSize, GraphicsUnit.Pixel);

            _year.Location = new Point((int)(Width * 0.1), (int)(Height * 0.35));
            _year.Size = new Size((int)(Width * 0.8), (int)(Height * 0.3));
            _year.Font = new Font(Program.MontserratBold, (int)(_year.Size.Height * 0.85), GraphicsUnit.Pixel);
        }
        
        ResizeLabels();
        SizeChanged += (_, _) => ResizeLabels();
        Click += (_, _) =>
        {
            if (!IsRevealed && Player.CurrentPlayer == Player.LocalPlayer && !Timeline.AllowTokenPlacement)
            {
                Task.Run(() => {
                    //Wenn das Lied vom Lokalem Spieler ist kann er das Lied Raten
                    var guess = MessageBox.Show("Frage", "Möchtest du den Track erraten?", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (guess == DialogResult.Yes)
                    {
                        var guessForm = new GuessForm();
                        guessForm.ShowDialog();
                    }

                    Player.LocalPlayer.ConfirmTrack();
                });
            }
        };
    }

    public void MarkAsConfirmed(bool wrong)
    {
        IsRevealed = true;
        IsCorrect = !wrong;
        BackgroundImage = null;
        BackColor = GetHashColor();
        _artist.Visible = true;
        _year.Visible = true;
        _title.Visible = true;
        if (wrong)
            _artist.ForeColor = _year.ForeColor = _title.ForeColor = Color.Red;
        Invalidate();
    }

    private Color GetHashColor()
    {
        var hash = 0;
        foreach (var c in Track.Id)
        {
            hash = c + ((hash << 5) - hash);
        }
        var colorStr = "#";
        for (var i = 0; i < 3; i++)
        {
            var value = (hash >> (i * 8)) & 0xff;
            colorStr += Convert.ToString(value, 16).PadLeft(2, '0');
        }
        
        var color = ColorTranslator.FromHtml(colorStr);
        if (color.GetBrightness() < 0.4f)
            color = Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);

        return color;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
    }
}