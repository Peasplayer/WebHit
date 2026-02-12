namespace Hitster;

public class Card : Panel
{
    private Color baseColor;
    public bool IsSelected { get; private set; }
    public bool IsPlaced { get; private set; } 

    public Card(Color color)
    {
        baseColor = color;
        Width = 80;
        Height = 80;
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
    }

    public void Deselect()
    {
        IsSelected = false;
        BackColor = baseColor;
    }

    public void MarkAsPlaced()
    {
        IsPlaced = true;
        IsSelected = false;
    }
}