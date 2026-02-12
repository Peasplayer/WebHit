namespace Hitster;

public partial class Form1 : Form
{
    private Timeline timeline;
    private FlowLayoutPanel handPanel;
    private Card selectedCard;

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
            Location = new Point(20, 230),
            Size = new Size(800, 120),
            BackColor = Color.DarkGray,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        Controls.Add(handPanel);

        for (int i = 0; i < 5; i++)
        {
            handPanel.Controls.Add(CreateHandCard());
        }
    }

    private Card CreateHandCard()
    {
        var card = new Card(Color.Gold);

        card.Click += (_, _) =>
        {
            if (selectedCard != null)
            {
                selectedCard.Deselect();
            }

            selectedCard = card;
            card.Select();
        };

        return card;
    }

    private void OnSlotClicked(int index)
    {
        if (selectedCard == null)
        {
            return;
        }

        handPanel.Controls.Remove(selectedCard);
        timeline.InsertCard(selectedCard, index);
        selectedCard = null;
    }
}