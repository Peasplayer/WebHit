namespace Hitster;

public class CardSlot : Panel
{
    public int Index { get; }

    private readonly int normalWidth;
    private readonly int expandedWidth;

    public event Action<int>? SlotClicked;

    public CardSlot(int index, int slotWidth, int cardSize)
    {
        Index = index;
        normalWidth = slotWidth;
        expandedWidth = cardSize;

        Width = slotWidth;
        Height = cardSize;
        Margin = new Padding(5);
        BackColor = Color.DeepPink;

        MouseEnter += (_, _) => Expand();
        MouseLeave += (_, _) => Collapse();
        Click += (_, _) => SlotClicked?.Invoke(Index);
    }

    private void Expand()
    {
        Width = expandedWidth;
        BackColor = Color.Orange;
    }

    private void Collapse()
    {
        Width = normalWidth;
        BackColor = Color.DeepPink;
    }
}