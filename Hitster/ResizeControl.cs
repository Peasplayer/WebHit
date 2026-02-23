namespace Hitster;

public struct ResizeControl
{
    public Control Control { get; }
    public SizeF Size { get; set; }
    public PointF Location { get; set; }
    
    public ResizeControl(Control control, SizeF size, PointF location)
    {
        Control = control;
        Size = size;
        Location = location;
    }
}