using Hitster.Networking;
using NAudio.Wave;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    private Timeline timeline;
    private Card? currentCard;
    private readonly Button _confirmButton;

    private WasapiOut _musicPlayer;
    
    public Form1()
    {
        InitializeComponent();
        
        timeline = new Timeline();
        timeline.SlotClicked += OnSlotClicked;
        RegisterResizeControl(timeline, new SizeF(30, 3.5f), new PointF(1, 1), timeline.AfterResize);
        Controls.Add(timeline);
        Task.Run(() =>
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var track = NetworkManager.Instance.RequestTrackData();
            watch.Stop();
            Console.WriteLine("Track took " + watch.ElapsedMilliseconds + " ms");
            timeline.Invoke(() =>
            {
                var card = new Card(track);
                timeline.InsertCard(card, 0);
                card.MarkAsConfirmed();
                timeline.Invalidate();
                RegisterResizeControl(card, new SizeF(3f, 3f));
            });
        });

        _confirmButton = new Button { Text = "Confirm", BackColor = Color.DarkGray };
        _confirmButton.Click += (_, _) =>
        {
            if (currentCard == null)
            {
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
                        currentCard = card;
                        timeline.InsertCard(card, 0);
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
                return;
            }
            
            if (_musicPlayer != null)
                _musicPlayer.Stop();
            
            currentCard.MarkAsConfirmed();
            currentCard = null;
            timeline.ToggleSlots(false);
        };
        Controls.Add(_confirmButton);
        RegisterResizeControl(_confirmButton, new Size(2, 2), new Point(1, 7));
    }

    private void OnSlotClicked(int index)
    {
        if (currentCard == null) return;

        timeline.InsertCard(currentCard, index);
    }
}