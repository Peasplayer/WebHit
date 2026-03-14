namespace Hitster;

public abstract class ResizeForm : Form
{
    private Panel _container;
    private int widthUnit;
    private int heightUnit;
    private List<ResizeControl> _resizeControls = new ();
    private event Action? _resized;

    public ResizeForm()
    {
        DoubleBuffered = true;
        
        _container = new Panel{BackColor = Color.Fuchsia};
        _container.SendToBack();
        Controls.Add(_container);
        
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
        _container.Width = calcWidth;
        _container.Height = calcHeight;
        _container.Location = new Point((ClientSize.Width - _container.Width) / 2, (ClientSize.Height - _container.Height) / 2);
        _container.SendToBack();
        
        widthUnit = _container.Width / 32;
        heightUnit = _container.Height / 18;

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
        return new Point((int)(_container.Location.X + location.X * widthUnit), (int)(_container.Location.Y + location.Y * heightUnit));
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