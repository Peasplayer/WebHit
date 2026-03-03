using Hitster.Networking;

namespace Hitster;

public class Timeline : Panel
{
    private List<Card> _cards = new();
    private readonly List<Panel> _activeSlots = new();
    private bool _slotsVisible;
    public Player? Player { get; private set; }

    public event Action<int>? SlotClicked;

    public Timeline()
    {
        BackColor = Color.Green;
        Render();
    }

    public void SetPlayer(Player player)
    {
        if (Player != null && Player.Id == player.Id)
        {
            return;
        }
        
        Player = player;
        
        foreach (var card in _cards)
        {
            Controls.Remove(card);
            card.Dispose();
        }
        foreach (var t in Player.AllTracks)
        {
            var card = new Card(t);
            _cards.Add(card);
        }
        
        Render();
    }

    public void UpdateTracks()
    {
        try
        {
            var cards = new List<Card>();
            foreach (var t in Player.AllTracks)
            {
                var card = _cards.Find(c => c.Track.Id == t.Id);
                if (card == null)
                {
                    card = new Card(t);
                    _cards.Add(card);
                }

                if (Player.CurrentTrack != t && !card.IsConfirmed)
                    card.MarkAsConfirmed();
                cards.Add(card);
            }

            _cards = cards;
            Render();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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

        var cardWidth = Height / 7 * 6;
        var totalWidth = _cards.Count * cardWidth;
        var startX = (Width - totalWidth) / 2;

        int slotIndex = 1;
        foreach (var card in _cards)
        {
            var cardIndex = _cards.IndexOf(card);
            card.Size = new Size(cardWidth, cardWidth);
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