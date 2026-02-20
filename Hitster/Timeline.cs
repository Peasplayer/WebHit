namespace Hitster;

public class Timeline
{
    public readonly FlowLayoutPanel panel; //Pannel in dem alles angeordnet wird
    private readonly List<Card> cards = new(); //Liste mit allen Karten

    //Größe der Karten
    private const int SlotWidth = 30;
    private const int CardSize = 80;

    private bool SlotsVisible;

    public event Action<int>? SlotClicked; //Übergibt den Index des gecklickten Slots
    public Timeline()
    {
        panel = new FlowLayoutPanel
        {
            BackColor = Color.Green,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };

        Render();
    }

    public void InsertCard(Card card, int index)
    {
        if (card.IsConfirmed)
            return;
        
        if (cards.Contains(card))
            cards.Remove(card);
        
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
        var slot = new Panel
        {
            Height = (int)(panel.Height * 0.9),
            Width = (int)(panel.Width / 60f),
            Margin = new Padding(0, (int)(panel.Height * 0.05), 0, (int)(panel.Height * 0.05)),
            BackColor = Color.DeepPink
        };
        
        slot.Click += (_, _) =>
        {
            SlotClicked?.Invoke(index);
        };
        panel.Controls.Add(slot);
    }
}