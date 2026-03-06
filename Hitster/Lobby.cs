using Hitster.Networking;

namespace Hitster;

public partial class Lobby : Form
{
    public static Lobby? Instance { get; private set; }
    
    private Button Startbtn;
    public Lobby()
    {
        Instance = this;
        FormClosing += (_, _) => Instance = null;
        InitializeComponent();
        
        Startbtn = new Button
        {
            Cursor = Cursors.Hand,
        };
        Startbtn.Click += (_, _) =>
        {
            NetworkManager.Instance.RpcStart();
        };
       Controls.Add(Startbtn);
    }
}