namespace Hitster;

public class Timeline : Panel
{
    private readonly List<Card> _cards = new();
    private readonly List<CardSlot> _activeSlots = new();
    private bool SlotsVisible;

    public event Action<int>? SlotClicked;
    
    public event Action? Resized;

    public Timeline()
    {
        BackColor = Color.Green;
        /*SizeChanged += (_, _) =>
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline SizeChanged");
            Render();
        };
        ParentChanged += (_, _) =>
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline ParentChanged");
            Render();
        };*/
        Resized += () =>
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline Resized");
            Render();
        };
        Click += (_, _) =>
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline Click");
            Render();
        };
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
        //card.SizeChanged -= OnCardSizeChanged; 
        //card.SizeChanged += OnCardSizeChanged;
        Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline InsertCard");
        Render();
    }

    private void OnCardSizeChanged(object? sender, EventArgs e)
    {
        Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline OnCardSizeChanged");
        Render();
    }

    public void ToggleSlots(bool showSlots)
    {
        SlotsVisible = showSlots;
        Render();
    }

    private void Render()
    {
        Console.WriteLine(DateTime.Now.ToLongTimeString() + " Timeline Render");
        Controls.Clear();
        
        if (Parent != null)
        {
            foreach (var slot in _activeSlots)
            {
                Parent.Controls.Remove(slot);
                slot.Dispose();
            }
            _activeSlots.Clear();
        }

        if (Width == 0 || Height == 0) 
        {
            return;
        }
        
        var totalWidth = _cards.Sum(card => card.Width);
        var startX = (Width - totalWidth) / 2;

        foreach (var card in _cards)
        {
            card.Location = new Point(startX + card.Width * _cards.IndexOf(card), 0);
            Controls.Add(card);
        }

        /*int cardWidth = _cards.Count > 0 ? _cards[0].Width : 80;
        int cardHeight = _cards.Count > 0 ? _cards[0].Height : 120;

        int gap = 0; //Kein Abstand zwoschen den Karten
        int totalContentWidth = (_cards.Count * cardWidth) + (Math.Max(0, _cards.Count - 1) * gap);
        
        int startX = (Width - totalContentWidth) / 2;
        int cardY = (Height - cardHeight) / 2;

        for (int i = 0; i <= _cards.Count; i++)
        {
            int slotCenterX;
            if (i == 0) slotCenterX = startX - gap / 2;
            else if (i == _cards.Count) slotCenterX = startX + totalContentWidth + gap / 2;
            else slotCenterX = startX + (i * cardWidth) + ((i - 1) * gap) + gap / 2;

            if (SlotsVisible && Parent != null)
            {
                bool isEnabled = false;
                if (_cards.Count == 0) isEnabled = true;
                else if (i == 0) isEnabled = _cards[0].IsConfirmed;
                else if (i == _cards.Count) isEnabled = _cards[i - 1].IsConfirmed;
                else isEnabled = _cards[i - 1].IsConfirmed && _cards[i].IsConfirmed;
                var slot = new CardSlot(i, 30, cardWidth, isEnabled);
                slot.SlotClicked += (index) => SlotClicked?.Invoke(index);
                int slotX = this.Location.X + slotCenterX - (slot.Width / 2);
                int slotY = this.Location.Y - slot.Height - 10; 
                slot.Location = new Point(slotX, slotY);
                _activeSlots.Add(slot);
                Parent.Controls.Add(slot);
                slot.BringToFront(); 
            }

            if (i < _cards.Count)
            {
                var card = _cards[i];
                card.Location = new Point(startX + i * (cardWidth + gap), 0);
                Controls.Add(card);
            }
        }*/
    }
}