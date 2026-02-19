using System.Drawing.Drawing2D;

namespace Hitster;

public class Card : Panel
{
    private Color baseColor;
    public bool IsSelected { get; private set; }
    public bool IsPlaced { get; private set; }
    public TrackData Track { get; }

    public Card(Color color, TrackData track)
    {
        baseColor = color;
        Track = track;

        Width = 120;
        Height = 160;

        BackColor = color;
        Margin = new Padding(5);
        BorderStyle = BorderStyle.FixedSingle;
        Click += OnClick;
    }

    private void OnClick(object? sender, EventArgs e)
    {
        if (IsPlaced)
        {
            return;
        }

        if (!IsSelected)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public void Select()
    {
        IsSelected = true;
        BackColor = Color.OrangeRed;
        Invalidate();
    }

    public void Deselect()
    {
        IsSelected = false;
        BackColor = baseColor;
        Invalidate();
    }

    public void MarkAsPlaced()
    {
        IsPlaced = true;
        Deselect();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        DrawArtist(g);   
        DrawYear(g);     
        DrawTitle(g);    
    }

    private void DrawArtist(Graphics g)
    {
        string text = Track.Artist;

        Font font = new Font("Arial", Width, FontStyle.Bold, GraphicsUnit.Pixel);
        var ratio = g.MeasureString(text, font);

        font = new Font("Arial",
            (int)(Width * 0.7 * ratio.Height / ratio.Width),
            FontStyle.Bold,
            GraphicsUnit.Pixel);

        SizeF textSize = g.MeasureString(text, font);

        float x = (Width - textSize.Width) / 2;
        float y = Height * 0.08f;

        g.DrawString(text, font, Brushes.White, x, y);
    }
    
    private void DrawYear(Graphics g)
    {
        string text = Track.ReleaseYear.ToString();

        Font font = new Font("Arial", Width, FontStyle.Bold, GraphicsUnit.Pixel);
        var ratio = g.MeasureString(text, font);

        font = new Font("Arial",
            (int)(Width * 1.1 * ratio.Height / ratio.Width), // größer als andere Texte
            FontStyle.Bold,
            GraphicsUnit.Pixel);

        SizeF textSize = g.MeasureString(text, font);

        float x = (Width - textSize.Width) / 2;
        float y = (Height - textSize.Height) / 2;

        g.DrawString(text, font, Brushes.Gold, x, y);
    }
    
    private void DrawTitle(Graphics g)
    {
        string text = Track.Name;

        Font font = new Font("Arial", Width, FontStyle.Regular, GraphicsUnit.Pixel);
        var ratio = g.MeasureString(text, font);

        font = new Font("Arial",
            (int)(Width * 0.6 * ratio.Height / ratio.Width),
            FontStyle.Regular,
            GraphicsUnit.Pixel);

        SizeF textSize = g.MeasureString(text, font);

        float x = (Width - textSize.Width) / 2;
        float y = Height - textSize.Height - Height * 0.08f;

        g.DrawString(text, font, Brushes.White, x, y);
    }
}