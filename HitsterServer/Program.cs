using Fleck;
using HitsterServer.MusicData;

namespace HitsterServer;

class Program
{
    const int Port = 9443;
    
    static void Main(string[] args)
    {
        // Der Server wird gestartet
        var server = new GameServer(Port);
        
        FleckLog.Info("Loading tracks...");
        var task = MusicManager.GetTracks();
        task.Wait();
        FleckLog.Info($"Done! Loaded {task.Result.Length} tracks");

        FleckLog.Warn("Press 'X' to exit the program");
        while (Console.ReadKey(true).Key != ConsoleKey.X);
    }
}