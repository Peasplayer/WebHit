namespace Hitster;

public abstract class ResizeForm : Form
{
    private Rectangle _container;
    private int widthUnit;
    private int heightUnit;

    public ResizeForm()
    {
        DoubleBuffered = true;
        WindowState = FormWindowState.Maximized; //macht Vollbild
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
        
        widthUnit = _container.Width / 32;
        heightUnit = _container.Height / 18;
        
        RenderForm();
    }

    protected abstract void RenderForm();

    enum Unit
    {
        Pixels,
        Percentage,
        Unit
    }
    
    protected Size GetSize(int width, int height)
    {
        return new Size(width * widthUnit, height * heightUnit);
    }

    protected Point GetLocation(int x, int y)
    {
        return new Point(_container.X + x * widthUnit, _container.Y + y * heightUnit);
    }
}