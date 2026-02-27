namespace Hitster;

public class Timeline : Panel
{
    private readonly List<Card> _cards = new();
    private readonly List<Panel> _activeSlots = new();
    private bool _slotsVisible;

    public event Action<int>? SlotClicked;

    public Timeline()
    {
        BackColor = Color.Green;
        Render();
    }

    public void InsertCard(Card card, int index)
    {
        if (card.IsConfirmed)
            return;
        
        if (_cards.Contains(card))
            _cards.Remove(card);
        
        if (index >= _cards.Count)
            _cards.Add(card);
        else
            _cards.Insert(index, card);
        
        Render();
    }

    public void AfterResize()
    {
        Render();
    }

    public void ToggleSlots(bool showSlots)
    {
        _slotsVisible = showSlots;
        Render();
    }

    private void Render()
    {
        foreach (var slot in _activeSlots)
        {
            Controls.Remove(slot);
            slot.Dispose();
        }
        _activeSlots.Clear();
        Controls.Clear();

        if (Width == 0 || Height == 0) 
        {
            return;
        }
        
        var totalWidth = _cards.Sum(card => card.Width);
        var startX = (Width - totalWidth) / 2;

        int slotIndex = 1;
        foreach (var card in _cards)
        {
            var cardIndex = _cards.IndexOf(card);
            card.Location = new Point(startX + card.Width * cardIndex, Height - card.Height);
            Controls.Add(card);

            if (!_slotsVisible)
                continue;
            
            if (cardIndex == 0 && card.IsConfirmed)
                CreateSlot(0, card.Location.X);
            if (card.IsConfirmed && (cardIndex == _cards.Count - 1 || _cards[cardIndex + 1].IsConfirmed))
                CreateSlot(slotIndex, card.Location.X + card.Size.Width);
            
            if (card.IsConfirmed)
                slotIndex++;
        }
    }

    private void CreateSlot(int index, int middle)
    {
        var slot = new Panel
        {
            Height = Height / 7,
            Width = (int)(Width / 50f),
            BackColor = Color.AliceBlue,
        };
        slot.Location = new Point(middle - slot.Width / 2, 0);
        slot.Click += (_, _) => SlotClicked?.Invoke(index);
        Controls.Add(slot);
        _activeSlots.Add(slot);
    }
}