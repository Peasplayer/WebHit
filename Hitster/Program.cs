using Hitster.Networking;
using Hitster.Networking.Packets;

namespace Hitster;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var net = new NetworkManager();
        net.Connect("ws://127.0.0.1:9443", "Dieter");
        
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}