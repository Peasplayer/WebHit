namespace Hitster;

public abstract class ResizeForm : Form
{
    public Panel ContentContainer { get; } //Container in dem alle Elemente plaziert werden
    private int widthUnit;
    private int heightUnit;
    private List<ResizeControl> _resizeControls = new (); //Liste für alle Elemente im Container, die sich automatisch anpassen
    private event Action? _resized;

    public ResizeForm()
    {
        DoubleBuffered = true; //Vermindert Flackern
        
        //Container erstellen und hinter allen anderen Elementen plazieren
        ContentContainer = new Panel();
        ContentContainer.SendToBack();
        Controls.Add(ContentContainer);
        
        //Jedesmal wenn die Größe des Forms verändert wird wird diese neu gerendert
        SizeChanged += (_, _) => _startRenderingForm();
        Resize += (_, _) => _startRenderingForm();
        Invalidated += (_, _) => _startRenderingForm();
        
        _startRenderingForm();
    }

    private void _startRenderingForm()
    {
        var calcWidth = ClientSize.Width; //Breiteneinheit für das Layout
        var calcHeight = ClientSize.Width * 9 / 16; //Höheneinheit für das Layout (16:9 bzw. 32:18)
        if (calcHeight > ClientSize.Height)
        {
            //Anhand der maximalen höhe des Forms das passende 16:9 Format berechnen 
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

        // Alle Controls bekommen ihre angepasste Größe und Position
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
        _resizeControls.Add(c); //Elemente der Liste hinzufügen
        _resized += resized;
        _startRenderingForm(); //Position berchnen damit die Elemente direkt angezeigt werden
    }
    
    private Size GetSize(SizeF size)
    {
        return new Size((int)(size.Width * widthUnit), (int)(size.Height * heightUnit));
    }

    private Point GetLocation(PointF location)
    {
        return new Point((int)(ContentContainer.Location.X + location.X * widthUnit), (int)(ContentContainer.Location.Y + location.Y * heightUnit));
    }
    
    //Datenhalter für anpassende Controls
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