namespace Hitster;

public class Timeline : FlowLayoutPanel
{
    private readonly List<Card> _cards = new(); //Liste mit allen Karten

    //Größe der Karten
    private const int SlotWidth = 30;
    private const int CardSize = 80;

    private bool SlotsVisible;

    public event Action<int>? SlotClicked; //Übergibt den Index des gecklickten Slots
    public Timeline()
    {
        BackColor = Color.Green;
        FlowDirection = FlowDirection.LeftToRight;
        WrapContents = false;

        SizeChanged += (_, _) => Render();
        Invalidated += (_, _) => Render();
        Render();
    }

    public void InsertCard(Card card, int index)
    {
        if (card.IsConfirmed)
            return;
        
        if (_cards.Contains(card))
            _cards.Remove(card);
        
        Console.WriteLine(index + " - " + _cards.Count);
        if (index >= _cards.Count)
            _cards.Add(card);
        else
            _cards.Insert(index, card);
        
        Render();
    }

    public void ToggleSlots(bool showSlots)
    {
        SlotsVisible = showSlots;
        Render();
    }

    private void Render()
    {
        Controls.Clear();

        var contentWidth = 0;
        for (int i = 0; i < _cards.Count; i++)
        {
            var card = _cards[i];
            if (i == 0)
            {
                var firstSlot = CreateSlot(0);
                firstSlot.Enabled = card.IsConfirmed;
                contentWidth += firstSlot.Width;
            }
                
            Controls.Add(card);
            contentWidth += card.Width;

            var slot = CreateSlot(i + 1);
            slot.Enabled = card.IsConfirmed && (i + 1 == _cards.Count || _cards[i + 1].IsConfirmed);
            contentWidth += slot.Width;
        }

        var width = 0;
        for (int i = 0; i < Controls.Count; i++)
        {
            var c = Controls[i];
            width += c.Width + c.Margin.Horizontal;
        }
        Padding = new Padding((Width - width) / 2, 0, (Width - width) / 2, 0);
    }

    private Panel CreateSlot(int index)
    {
        var slot = new Panel
        {
            Height = Height,
            Width = (int)(Width / 60f),
            BackColor = Color.DeepPink,
            Tag = index
        };
        slot.Click += (_, _) => SlotClicked?.Invoke(index);
        Controls.Add(slot);
        return slot;
    }
}