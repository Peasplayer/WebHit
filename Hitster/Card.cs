using System.Drawing.Drawing2D;

namespace Hitster;

public sealed class Card : Panel
{
    public bool IsConfirmed { get; private set; }
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
        Margin = new Padding(5, 0, 5, 0);
        BorderStyle = BorderStyle.FixedSingle;

        _artist = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = Track.Artist, Visible = false, BackColor = Color.BlueViolet };
        _year = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = Track.ReleaseYear.ToString(), Visible = false, BackColor = Color.Coral };
        _title = new Label { TextAlign = ContentAlignment.MiddleCenter, Text = Track.Name, Visible = false, BackColor = Color.BlueViolet };
        Controls.AddRange(_artist, _year, _title);

        void ResizeLabels()
        {
            if (Height == 0 || Width == 0)
                return;
            
            _artist.Location = new Point((int)(Width * 0.05), 0);
            _artist.Size = new Size((int)(Width * 0.9), (int)(Height * 0.35));
            _artist.Font = new Font(Program.MontserratSemiBold, (int)(_artist.Size.Height * 0.7 / 3), GraphicsUnit.Pixel);

            _year.Location = new Point((int)(Width * 0.1), (int)(Height * 0.35));
            _year.Size = new Size((int)(Width * 0.8), (int)(Height * 0.3));
            _year.Font = new Font(Program.MontserratBold, (int)(_year.Size.Height * 0.9), GraphicsUnit.Pixel);

            _title.Location = new Point((int)(Width * 0.05), (int)(Height * 0.65));
            _title.Size = new Size((int)(Width * 0.9), (int)(Height * 0.35));
            _title.Font = new Font(Program.MontserratMediumItalic, (int)(_artist.Size.Height * 0.7 / 3), GraphicsUnit.Pixel);
        }
        
        ResizeLabels();
        SizeChanged += (_, _) => ResizeLabels();
    }

    public void MarkAsConfirmed()
    {
        IsConfirmed = true;
        BackgroundImage = null;
        BackColor = GetHashColor();
        _artist.Visible = true;
        _year.Visible = true;
        _title.Visible = true;
        Invalidate();
    }

    private Color GetHashColor()
    {
        var hash = 0;
        foreach (var c in Track.Id)
        {
            hash = c + ((hash << 5) - hash);
        }
        var color = "#";
        for (var i = 0; i < 3; i++)
        {
            var value = (hash >> (i * 8)) & 0xff;
            color += Convert.ToString(value, 16).PadLeft(2, '0');
        }
        return ColorTranslator.FromHtml(color);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
    }
}