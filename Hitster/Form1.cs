using Hitster.Networking;

namespace Hitster;

public partial class Form1 : ResizeForm
{
    private Timeline timeline;
    private FlowLayoutPanel handPanel;
    private Card? selectedCard;

    public Form1()
    {
        InitializeComponent();
        CreateTimeline();
        CreateHand();
    }

    private void CreateTimeline()
    {
        timeline = new Timeline(this);
        timeline.SlotClicked += OnSlotClicked;
    }

    private void CreateHand()
    {
        handPanel = new FlowLayoutPanel
        {
            BackColor = Color.DarkGray,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        handPanel.MouseDoubleClick += (sender, args) =>
        {
            Task.Run(() =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var track = NetworkManager.Instance.RequestTrackData();
                watch.Stop();
                Console.WriteLine("Track took " + watch.ElapsedMilliseconds + " ms");
                handPanel.Invoke(() =>
                {
                    handPanel.Controls.Add(CreateHandCard(track));
                });
            });
        };

        RegisterResizeControl(handPanel, new Size(10, 2), new Point(1, 4));
        Controls.Add(handPanel);
    }

    private Card CreateHandCard(TrackData track)
    {
        var card = new Card(Color.Black, track);

        void CardOnClick()
        {
            if (card.IsPlaced) return;

            if (selectedCard == card)
            {
                timeline.ToggleSlots(false);
                
                selectedCard.Deselect();
                selectedCard = null;
            }
            else
            {
                timeline.ToggleSlots(true);
                
                if (selectedCard != null)
                    selectedCard.Deselect();
                
                selectedCard = card;
                card.Select();
            }
        };
        
        card.MouseClick += (_, _) => CardOnClick();
        card.MouseDoubleClick += (_, _) => CardOnClick();

        return card;
    }

    private void OnSlotClicked(int index)
    {
        if (selectedCard == null) return;

        handPanel.Controls.Remove(selectedCard);
        timeline.InsertCard(selectedCard, index);
        timeline.ToggleSlots(false);
        selectedCard.MarkAsPlaced();
        selectedCard = null;
    }
}