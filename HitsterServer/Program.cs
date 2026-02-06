using Fleck;

namespace HitsterServer;

class Program
{
    const int Port = 9443;
    
    static void Main(string[] args)
    {
        // Der Server wird gestartet
        var server = new GameServer(Port);

        FleckLog.Warn("Press 'X' to exit the program");
        while (Console.ReadKey(true).Key != ConsoleKey.X);
    }
}