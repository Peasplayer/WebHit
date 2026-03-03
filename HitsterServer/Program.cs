using Fleck;
using HitsterServer.MusicData;
using HitsterServer.Packets;

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
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.X)
                break;
            if (key == ConsoleKey.S)
            {
                foreach (var pair in GameServer.Instance.Clients)
                {
                    Task.Run(async () =>
                    {
                        var packet = new TrackPacket(await MusicManager.GetRandomTrack(), pair.Value.Id);
                        GameServer.Instance.SendPacketEveryone(packet);
                    });
                }
            }
        }
    }
}