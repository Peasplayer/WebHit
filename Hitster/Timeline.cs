using Hitster.Networking;

namespace Hitster;

public class Timeline : Panel
{
    private static List<Timeline> _timelines = new List<Timeline>();
    public static bool AllowTokenPlacement { get; private set; }

    public static void UpdateTimeline(Player p)
    {
        foreach (var t in _timelines)
        {
            if (t._player?.Id == p.Id)
                t.Invoke(t.UpdateTracks);
        }
    }

    public static void ToggleTokenPlacement(bool allow)
    {
        AllowTokenPlacement = allow;
        foreach (var t in _timelines)
        {
            t.Invoke(t.UpdateTracks);
        }
    }
    
    public static void RevealTrack(Player p, TrackData track)
    {
        foreach (var t in _timelines)
        {
            if (t._player?.Id == p.Id)
                t.Invoke(() => t.RevealTrack(track));
        }
    }
    
    private List<Card> _cards = new();
    private readonly List<Panel> _activeSlots = new();
    private Player? _player;
    private Label _nameLabel;

    public Timeline()
    {
        _timelines.Add(this);
        BackColor = Color.DarkOliveGreen;
        _nameLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 0)
        };
        Render();
    }

    public void SetPlayer(Player? player)
    {
        if (player == null || _player?.Id == player.Id)
            return;
        
        _player = player;
        
        foreach (var card in _cards)
        {
            Controls.Remove(card);
            card.Dispose();
        }
        _cards.Clear();
        foreach (var t in _player.AllTracks)
        {
            var card = new Card(t);
            if (_player.CurrentTrack != t)
                card.MarkAsRevealed(false);
            _cards.Add(card);
        }
        
        Render();
    }

    private void RevealTrack(TrackData track)
    {
        var cardIndex = _cards.FindIndex(c => c.Track == track);
        var card = _cards[cardIndex];
        var wrong =
            (cardIndex != 0 && track.ReleaseYear < _cards[cardIndex - 1].Track.ReleaseYear) ||
            (cardIndex < _cards.Count - 1 &&
             track.ReleaseYear > _cards[cardIndex + 1].Track.ReleaseYear);
            card.MarkAsRevealed(wrong); //Karte wird aufgedeckt und wird rot
        if (wrong)
        {
            // Lokaler Spieler sucht gesetzten, Token der richtig ist und übergibt den Track
            if (Player.LocalPlayer == _player)
            {
                foreach (var guess in Player.TokenGuesses)
                {
                    var guessedWrong =
                        (guess.Value != 0 && track.ReleaseYear < _cards[guess.Value - 1].Track.ReleaseYear) ||
                        (guess.Value < _cards.Count - 1 &&
                         track.ReleaseYear > _cards[guess.Value + 1].Track.ReleaseYear);
                    
                    if (!guessedWrong)
                    {
                        NetworkManager.RpcTokenCorrect(Player.GetPlayer(guess.Key), track);
                        break;
                    }
                }
                Player.TokenGuesses.Clear();
            }
            
            // Entfernt die Karte nach 3 Sekunden wenn sie falsch ist
            Task.Run(() =>
            {
                Task.Delay(3000).Wait();
                _player?.LooseTrack(track);
            });
        }
        
        CheckForWin();
        Render();
    }

    private void UpdateTracks()
    {
        if (_player == null)
            return;
        
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
                
                if (!card.IsRevealed && _player.CurrentTrack != t)
                    card.MarkAsRevealed(false);

                cards.Add(card);
            }

            // Tracks die wieder entfernt wurden (weil falsch eingeordnet) werden auch aus der Timeline gelöscht
            foreach (var card in _cards.ToList())
            {
                if (!_player.AllTracks.Contains(card.Track))
                {
                    _cards.Remove(card);
                    Controls.Remove(card);
                    card.Dispose();
                }
            }

            _cards = cards;
            CheckForWin();
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
        if (_player == null)
            return;
        
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
        
        _nameLabel.Text = (_player == Player.LocalPlayer ? "Deine" : _player.Name + "s") + " Timeline";
        _nameLabel.Size = new Size(Size.Width, Size.Height / 8);
        _nameLabel.Font = new Font(Program.MontserratSemiBold, (int)(_nameLabel.Size.Height * 0.85), GraphicsUnit.Pixel);
        Controls.Add(_nameLabel);

        var cardWidth = Height / 8 * 6;
        var totalWidth = _cards.Count * cardWidth;
        var startX = (Width - totalWidth) / 2;

        int slotIndex = 1;
        foreach (var card in _cards)
        {
            var cardIndex = _cards.IndexOf(card);
            card.Size = new Size(cardWidth, cardWidth);
            card.Location = new Point(startX + card.Width * cardIndex, Height - card.Height);
            Controls.Add(card);

            if ((_player == Player.LocalPlayer && Player.CurrentPlayer == Player.LocalPlayer &&
                 _cards.Find(c => !c.IsRevealed) != null && !AllowTokenPlacement) || 
                (_player != Player.LocalPlayer && _player == Player.CurrentPlayer && AllowTokenPlacement 
                 && Player.LocalPlayer.Tokens > 0 && !Player.TokenGuesses.ContainsKey(Player.LocalPlayer.Id)))
            {
                if (cardIndex == 0 && card.IsRevealed && (!AllowTokenPlacement || !Player.TokenGuesses.ContainsValue(cardIndex)))
                    CreateSlot(0, card.Location.X);
                if (card.IsRevealed && (cardIndex == _cards.Count - 1 || _cards[cardIndex + 1].IsRevealed) && (!AllowTokenPlacement || !Player.TokenGuesses.ContainsValue(cardIndex)))
                    CreateSlot(slotIndex, card.Location.X + card.Size.Width);
            
                if (card.IsRevealed)
                    slotIndex++;
            }
        }
    }

    private void CheckForWin()
    {
        var count = _cards.FindAll(c => c.IsRevealed && c.IsCorrect).Count;
        if (count == Settings.CurrentSettings.RequiredCards && Player.LocalPlayer == _player)
        {
            NetworkManager.RpcPlayerWon(_player);
        }
    }

    private void CreateSlot(int index, int middle)
    {
        var slot = new Panel
        {
            BackgroundImage = Image.FromStream(Program.GetResource("Pfeil.png")),
            BackgroundImageLayout = ImageLayout.Stretch,
            Height = Height / 8,
            Width = (int)(Width / 50f),
            BackColor = Color.Transparent
        };
        slot.Location = new Point(middle - slot.Width / 2, Height / 8);
        slot.Click += (_, _) =>
        {
            if (AllowTokenPlacement)
                NetworkManager.RpcPlaceToken(index);
            else
                NetworkManager.RpcMoveCurrentTrack(index);
        };
        Controls.Add(slot);
        _activeSlots.Add(slot);
    }
}