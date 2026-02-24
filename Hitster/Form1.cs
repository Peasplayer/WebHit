using Hitster.Networking;
using NAudio.Wave;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    private Timeline timeline;
    private FlowLayoutPanel handPanel;
    private Card? currentCard;
    private readonly Button _confirmButton;

    private WasapiOut _musicPlayer;
    
    public Form1()
    {
        InitializeComponent();
        
        timeline = new Timeline();
        timeline.SlotClicked += OnSlotClicked;
        RegisterResizeControl(timeline, new Size(30, 3), new Point(1, 1));
        Controls.Add(timeline);
        Task.Run(() =>
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var track = NetworkManager.Instance.RequestTrackData();
            watch.Stop();
            Console.WriteLine("Track took " + watch.ElapsedMilliseconds + " ms");
            timeline.Invoke(() =>
            {
                timeline.ToggleSlots(true);
                var card = new Card(track);
                timeline.InsertCard(card, 0);
                card.MarkAsConfirmed();
                timeline.Invalidate();
                RegisterResizeControl(card, new SizeF(3f, 3f));
            });
        });
        
        CreateHand();

        _confirmButton = new Button { Text = "Confirm", BackColor = Color.DarkGray, Enabled = false };
        _confirmButton.Click += (_, _) =>
        {
            if (currentCard == null) return;
            
            if (_musicPlayer != null)
                _musicPlayer.Stop();
            
            currentCard.MarkAsConfirmed();
            currentCard = null;
            timeline.ToggleSlots(false);
            _confirmButton.Enabled = false;
        };
        Controls.Add(_confirmButton);
        RegisterResizeControl(_confirmButton, new Size(2, 2), new Point(1, 7));
    }

    private void CreateHand()
    {
        handPanel = new FlowLayoutPanel
        {
            BackColor = Color.DarkGray,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        handPanel.MouseDoubleClick += (_, _) =>
        {
            Task.Run(() =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var track = NetworkManager.Instance.RequestTrackData();
                watch.Stop();
                Console.WriteLine("Track took " + watch.ElapsedMilliseconds + " ms");
                handPanel.Invoke(() =>
                {
                    timeline.ToggleSlots(true);
                    var card = new Card(track);
                    currentCard = card;
                    handPanel.Controls.Add(card);
                    RegisterResizeControl(card, new SizeF(3f, 3f));
                });
                
                using(var mf = new MediaFoundationReader(track.Link))
                {
                    _musicPlayer = new WasapiOut();
                    _musicPlayer.Init(mf);
                    _musicPlayer.Play();
                    while (_musicPlayer.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
        };

        RegisterResizeControl(handPanel, new Size(10, 2), new Point(1, 4));
        Controls.Add(handPanel);
    }

    private void OnSlotClicked(int index)
    {
        if (currentCard == null) return;

        handPanel.Controls.Remove(currentCard);
        timeline.InsertCard(currentCard, index);
        _confirmButton.Enabled = true;
    }
}