namespace Hitster;

public class Card : Panel
{
    public bool IsSelected { get; private set; }

    public Card(Color color)
    {
        Width = 80;
        Height = 80;
        BackColor = color;
        Margin = new Padding(5);
        BorderStyle = BorderStyle.FixedSingle;
    }

    public void Select()
    {
        IsSelected = true;
        BackColor = Color.OrangeRed;
    }

    public void Deselect(Color baseColor)
    {
        IsSelected = false;
        BackColor = baseColor;
    }
}