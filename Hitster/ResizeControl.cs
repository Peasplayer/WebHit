namespace Hitster;

public struct ResizeControl
{
    public Control Control { get; }
    public Size Size { get; set; }
    public Point Location { get; set; }
    
    public ResizeControl(Control control, Size size, Point location)
    {
        Control = control;
        Size = size;
        Location = location;
    }
}