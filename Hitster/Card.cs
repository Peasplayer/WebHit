using System.Drawing.Drawing2D;

namespace Hitster;

public sealed class Card : Panel
{
    public bool IsConfirmed { get; private set; }
    public TrackData Track { get; }

    private TextBox _artist;
    private TextBox _year;
    private TextBox _title;

    public Card(TrackData track)
    {
        Track = track;

        Width = Height = 160;
        BackColor = Color.Black;
        BackgroundImage = Image.FromStream(Program.GetResource("Karte.jpg"));
        BackgroundImageLayout = ImageLayout.Zoom;
        Margin = new Padding(5);
        BorderStyle = BorderStyle.FixedSingle;

        _artist = new TextBox();
        _year = new TextBox();
        _title = new TextBox();
    }

    public void MarkAsConfirmed()
    {
        IsConfirmed = true;
        BackgroundImage = null;
        BackColor = GetHashColor();
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

        if (IsConfirmed)
        {
            DrawArtist(g);   
            DrawYear(g);     
            DrawTitle(g);
        } 
    }

    private void DrawArtist(Graphics g)
    {
        string text = Track.Artist;

        Font font = new Font(Program.MontserratSemiBold, Width, FontStyle.Bold, GraphicsUnit.Pixel);
        var ratio = g.MeasureString(text, font);

        font = new Font(Program.MontserratSemiBold,
            (int)(Width * 0.5 * ratio.Height / ratio.Width),
            FontStyle.Bold,
            GraphicsUnit.Pixel);

        SizeF textSize = g.MeasureString(text, font);

        float x = (Width - textSize.Width) / 2;
        float y = Height * 0.08f;

        g.DrawString(text, font, Brushes.Black, x, y);
    }
    
    private void DrawYear(Graphics g)
    {
        string text = Track.ReleaseYear.ToString();

        Font font = new Font(Program.MontserratBold, Width, FontStyle.Bold, GraphicsUnit.Pixel);
        var ratio = g.MeasureString(text, font);

        font = new Font(Program.MontserratBold,
            (int)(Width * 0.85 * ratio.Height / ratio.Width), // größer als andere Texte
            FontStyle.Bold,
            GraphicsUnit.Pixel);

        SizeF textSize = g.MeasureString(text, font);

        float x = (Width - textSize.Width) / 2;
        float y = (Height - textSize.Height) / 2;

        g.DrawString(text, font, Brushes.Black, x, y);
    }
    
    private void DrawTitle(Graphics g)
    {
        string text = Track.Name;

        Font font = new Font(Program.MontserratMediumItalic, Width, FontStyle.Regular, GraphicsUnit.Pixel);
        var ratio = g.MeasureString(text, font);

        font = new Font(Program.MontserratMediumItalic,
            (int)(Width * 0.55 * ratio.Height / ratio.Width),
            FontStyle.Regular,
            GraphicsUnit.Pixel);

        SizeF textSize = g.MeasureString(text, font);

        float x = (Width - textSize.Width) / 2;
        float y = Height - textSize.Height - Height * 0.08f;

        g.DrawString(text, font, Brushes.Black, x, y);
    }
}