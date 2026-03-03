using Hitster.Networking;

namespace Hitster;

public class Timeline : Panel
{
    private static List<Timeline> _timelines = new List<Timeline>();

    public static void UpdateTimeline(Player p)
    {
        foreach (var t in _timelines)
        {
            if (t._player?.Id == p.Id)
                t.Invoke(t.UpdateTracks);
        }
    }
    
    private List<Card> _cards = new();
    private readonly List<Panel> _activeSlots = new();
    private Player? _player;

    public Timeline()
    {
        _timelines.Add(this);
        BackColor = Color.Green;
        Render();
    }

    public void SetPlayer(Player player)
    {
        if (_player != null && _player.Id == player.Id)
        {
            return;
        }
        
        _player = player;
        
        foreach (var card in _cards)
        {
            Controls.Remove(card);
            card.Dispose();
        }
        foreach (var t in _player.AllTracks)
        {
            var card = new Card(t);
            _cards.Add(card);
        }
        
        Render();
    }

    private void UpdateTracks()
    {
        try
        {
            var cards = new List<Card>();
            foreach (var t in _player.AllTracks)
            {
                var card = _cards.Find(c => c.Track.Id == t.Id);
                if (card == null)
                {
                    card = new Card(t);
                    _cards.Add(card);
                }

                if (_player.CurrentTrack != t && !card.IsConfirmed)
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

            if (_player != Player.LocalPlayer || Player.CurrentPlayer != Player.LocalPlayer || Player.LocalPlayer.CurrentTrack == null)
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
        slot.Click += (_, _) => NetworkManager.Instance.RpcMoveCurrentTrack(index);
        Controls.Add(slot);
        _activeSlots.Add(slot);
    }
}