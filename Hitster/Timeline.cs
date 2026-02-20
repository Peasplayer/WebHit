namespace Hitster;

public class Timeline
{
    private readonly FlowLayoutPanel panel; //Pannel in dem alles angeordnet wird
    private readonly List<Card> cards = new(); //Liste mit allen Karten

    //Größe der Karten
    private const int SlotWidth = 30;
    private const int CardSize = 80;

    private bool SlotsVisible;

    public event Action<int>? SlotClicked; //Übergibt den Index des gecklickten Slots
    public Timeline(ResizeForm parent)
    {
        panel = new FlowLayoutPanel
        {
            BackColor = Color.Green,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };

        parent.RegisterResizeControl(panel, new Size(10, 2), new Point(1, 1));
        parent.Controls.Add(panel);
        Render();
    }

    public void InsertCard(Card card, int index)
    {
        if (cards.Contains(card))
            cards.Remove(card);

        if (index < 0)
            index = 0;

        if (index > cards.Count)
            index = cards.Count;

        cards.Insert(index, card);
        Render();
    }

    public void ToggleSlots(bool showSlots)
    {
        SlotsVisible = showSlots;
        Render();
    }

    private void Render()
    {
        panel.Controls.Clear();
        
        for (int i = 0; i < cards.Count; i++)
        {
            AddSlot(i);
            panel.Controls.Add(cards[i]);
        }
        
        AddSlot(cards.Count);

        var width = 0;
        for (int i = 0; i < panel.Controls.Count; i++)
        {
            var c = panel.Controls[i];
            width += c.Width + c.Margin.Horizontal;
        }
        panel.Padding = new Padding((panel.Width - width) / 2, 0, (panel.Width - width) / 2, 0);
    }

    private void AddSlot(int index)
    {
        var slot = new CardSlot(index, SlotWidth, CardSize, SlotsVisible);
        slot.SlotClicked += i => SlotClicked?.Invoke(i);
        panel.Controls.Add(slot);
    }
}