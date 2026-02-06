namespace Hitster;

public partial class Form1 : Form
{
    private FlowLayoutPanel timelinePanel;
    public Form1()
    {
        InitializeComponent();
        TimeLineContainer();
        Test();
    }

    private void TimeLineContainer()
    {
        timelinePanel = new FlowLayoutPanel();
        timelinePanel.Location = new Point(20, 50);
        timelinePanel.Size = new Size(800, 160);
        timelinePanel.BackColor = Color.Green;
        Controls.Add(timelinePanel);
    }

    private void Test()
    {
        int slotWidth = 30;
        int cardSize = 80;

        for (int i = 0; i < 5; i++)
        {
            AddSlot(slotWidth, cardSize);
            AddCard(cardSize);
        }
        AddSlot(slotWidth, cardSize);
    }

    public void AddSlot(int slotWidth, int cardSize)
    {
        var slot = new Panel
        {
            Width = slotWidth,
            Height = cardSize,
            BackColor = Color.DeepPink,
            Margin = new Padding(5)
        };
        var animateSlot = false;
        var diff = cardSize - slotWidth;
        var steps = 10;
        var incDiff = diff / steps;
        var duration = 100;
        slot.MouseEnter += (s, e) =>
        {
            animateSlot = true;
            var t = Task.Run(() =>
            {
                for (int i = 0; i < steps; i++)
                {
                    if (animateSlot)
                    {
                        slot.Width += incDiff;
                        Task.Delay(duration / steps).Wait();
                    }
                    else
                    {
                        break;
                    }
                }
            });
        };
        slot.MouseLeave += (s, e) =>
        {
            var steps = (slot.Width - slotWidth) / incDiff;
            var t = Task.Run(() =>
            {
                for (int i = 0; i < steps; i++)
                {
                    slot.Width -= incDiff;
                    Task.Delay(duration / steps).Wait();
                }
            });
            animateSlot = false;
        };
        timelinePanel.Controls.Add(slot);
    }
    public void AddCard(int cardSize)
    {
        var card = new Panel
        {
            Width = cardSize,
            Height = cardSize,
            BackColor = Color.BlueViolet,
            Margin = new Padding(5)
        };
        timelinePanel.Controls.Add(card);
    }
}