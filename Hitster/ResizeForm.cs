namespace Hitster;

public abstract class ResizeForm : Form
{
    public Panel ContentContainer { get; } //Container in dem alle Elemente plaziert werden
    private int widthUnit;
    private int heightUnit;
    private List<ResizeControl> _resizeControls = new ();
    private event Action? _resized;

    public ResizeForm()
    {
        DoubleBuffered = true;
        
        //Container erstellen und hinter allen anderen Elementen plazieren
        ContentContainer = new Panel();
        ContentContainer.SendToBack();
        Controls.Add(ContentContainer);
        
        SizeChanged += (_, _) => _startRenderingForm();
        Resize += (_, _) => _startRenderingForm();
        Invalidated += (_, _) => _startRenderingForm();
        
        _startRenderingForm();
    }

    private void _startRenderingForm()
    {
        var calcWidth = ClientSize.Width;
        var calcHeight = ClientSize.Width * 9 / 16;
        if (calcHeight > ClientSize.Height)
        {
            calcWidth = ClientSize.Height * 16 / 9;
            calcHeight = ClientSize.Height;
        }
        
        // Setze Größe und Position des Containers zentriert im Fenster
        ContentContainer.Width = calcWidth;
        ContentContainer.Height = calcHeight;
        ContentContainer.Location = new Point((ClientSize.Width - ContentContainer.Width) / 2, (ClientSize.Height - ContentContainer.Height) / 2);
        ContentContainer.SendToBack();
        
        //Maßen einer Grid Einheit berechnen
        widthUnit = ContentContainer.Width / 32;
        heightUnit = ContentContainer.Height / 18;

        foreach (var c in _resizeControls)
        {
            c.Control.Size = GetSize(c.Size);
            if (c.Location != null)
                c.Control.Location = GetLocation(c.Location.Value);
        }
        
        if (_resized != null)
            _resized.Invoke();
    }

    public void RegisterResizeControl(Control control, SizeF size, PointF? location = null, Action? resized = null)
    {
        var c = new ResizeControl(control, size, location);
        _resizeControls.Add(c);
        _resized += resized;
        _startRenderingForm();
    }
    
    private Size GetSize(SizeF size)
    {
        return new Size((int)(size.Width * widthUnit), (int)(size.Height * heightUnit));
    }

    private Point GetLocation(PointF location)
    {
        return new Point((int)(ContentContainer.Location.X + location.X * widthUnit), (int)(ContentContainer.Location.Y + location.Y * heightUnit));
    }
    
    private struct ResizeControl
    {
        public Control Control { get; }
        public SizeF Size { get; set; }
        public PointF? Location { get; set; }
    
        public ResizeControl(Control control, SizeF size, PointF? location = null)
        {
            Control = control;
            Size = size;
            Location = location;
        }
    }
}