namespace Hitster;

public partial class Form1 : ResizeForm
{
    private Timeline timeline;
    private FlowLayoutPanel handPanel;
    private Card? selectedCard;

    public Form1()
    {
        InitializeComponent();
    }

    protected override void RenderForm()
    {
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
            Location = GetLocation(1, 1),
            Size = GetSize(10, 2),
            BackColor = Color.DarkGray,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        Controls.Add(handPanel);

        for (int i = 0; i < 5; i++)
        {
            handPanel.Controls.Add(CreateHandCard(i));
        }
    }

    private Card CreateHandCard(int index)
    {
        var track = new TrackData(index.ToString(), "Song " + (index + 1), "Artist " + (index + 1), "", 1980 + index);
        var card = new Card(Color.Black, track);

        card.Click += (_, _) =>
        {
            if (card.IsPlaced) return;

            if (selectedCard != null && selectedCard != card)
                selectedCard.Deselect();

            if (selectedCard == card)
            {
                selectedCard.Deselect();
                selectedCard = null;
            }
            else
            {
                selectedCard = card;
                card.Select();
            }
        };

        return card;
    }

    private void OnSlotClicked(int index)
    {
        if (selectedCard == null) return;

        handPanel.Controls.Remove(selectedCard);
        timeline.InsertCard(selectedCard, index);
        selectedCard.MarkAsPlaced();
        selectedCard = null;
    }
}