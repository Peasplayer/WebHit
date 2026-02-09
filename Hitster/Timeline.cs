namespace Hitster;

public class Timeline
{
    private readonly FlowLayoutPanel panel; //Pannel in dem alles angeordnet wird
    private readonly List<Card> cards = new(); //Liste mit allen Karten

    //Größe der Karten
    private const int SlotWidth = 30;
    private const int CardSize = 80;

    public event Action<int>? SlotClicked; //Übergibt den Index des gecklickten Slots
    public Timeline(Control parent)
    {
        panel = new FlowLayoutPanel
        {
            Location = new Point(20, 50),
            Size = new Size(800, 160),
            BackColor = Color.Green,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        parent.Controls.Add(panel);
        Render();
    }

    public void InsertCard(Card card, int index)
    {
        cards.Insert(index, card);
        Render();
    }

    private void Render()
    {
        panel.Controls.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            AddSlot(i);
            panel.Controls.Add(cards[i]);
        }

        AddSlot(cards.Count);
    }

    private void AddSlot(int index)
    {
        var slot = new CardSlot(index, SlotWidth, CardSize);
        slot.SlotClicked += i => SlotClicked?.Invoke(i);
        panel.Controls.Add(slot);
    }
}