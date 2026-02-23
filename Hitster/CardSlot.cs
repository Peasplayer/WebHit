namespace Hitster;

public class CardSlot : Panel
{
    public int Index { get; }

    private readonly int normalWidth;
    private readonly int expandedWidth;

    public event Action<int>? SlotClicked;

    public CardSlot(int index, int slotWidth, int cardSize, bool visible)
    {
        Index = index;
        normalWidth = slotWidth;
        expandedWidth = cardSize;

        Width = slotWidth;
        Height = cardSize;
        Margin = new Padding(5);
        BackColor = visible ? Color.DeepPink : Color.Transparent;

        if (visible)
        {
            MouseEnter += (_, _) => BackColor = Color.Orange;
            MouseLeave += (_, _) => BackColor = Color.DeepPink;
            Click += (_, _) => SlotClicked?.Invoke(Index);
        }
    }
}