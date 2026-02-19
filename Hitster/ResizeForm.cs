namespace Hitster;

public abstract class ResizeForm : Form
{
    private Rectangle _container;
    private Panel _containerDisplay;
    private int widthUnit;
    private int heightUnit;
    private List<ResizeControl> _resizeControls = new ();

    public ResizeForm()
    {
        DoubleBuffered = true;
        WindowState = FormWindowState.Maximized; //macht Vollbild
        
        _containerDisplay = new Panel{BackColor = Color.Fuchsia};
        _containerDisplay.SendToBack();
        Controls.Add(_containerDisplay);
        
        SizeChanged += (_, _) =>
        {
            _startRenderingForm();
        };
        
        _startRenderingForm();
    }

    private void _startRenderingForm()
    {
        _container = new Rectangle();
        
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
        
        _containerDisplay.Width = _container.Width;
        _containerDisplay.Height = _container.Height;
        _containerDisplay.Location = _container.Location;
        _containerDisplay.SendToBack();
        
        widthUnit = _container.Width / 32;
        heightUnit = _container.Height / 18;

        foreach (var c in _resizeControls)
        {
            c.Control.Size = GetSize(c.Size);
            c.Control.Location = GetLocation(c.Location);
        }
    }

    public void RegisterResizeControl(Control control, Size size, Point location)
    {
        var c = new ResizeControl(control, size, location);
        _resizeControls.Add(c);
    }
    
    private Size GetSize(Size size)
    {
        return new Size(size.Width * widthUnit, size.Height * heightUnit);
    }

    private Point GetLocation(Point location)
    {
        return new Point(_container.X + location.X * widthUnit, _container.Y + location.Y * heightUnit);
    }
}