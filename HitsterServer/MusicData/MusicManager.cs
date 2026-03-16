using System.Net.Http.Headers;
using Fleck;
using Newtonsoft.Json;

namespace HitsterServer.MusicData;

public class MusicManager
{
    private static DateTime _time;
    private static List<TrackData> _tracks = new ();
    private static List<string> _usedTracks = new ();
    
    public static async Task<List<TrackData>> LoadTracks()
    {
        if (_time + TimeSpan.FromMinutes(15) < DateTime.Now || _tracks.Count == 0)
        {
            FleckLog.Debug("Reloading expired track data...");
            using (var client = new HttpClient())
            {
                var rawApiData = //await new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("HitsterServer.json.txt")).ReadToEndAsync();
                await client.GetStringAsync($"https://api.deezer.com/playlist/{Packs[Settings.CurrentSettings.Pack]}/");
                _time = DateTime.Now;

                var rawTrackData = JsonConvert.DeserializeAnonymousType(rawApiData, new
                {
                    tracks =
                        new
                        {
                            data = new[] { new { id = "", title_short = "", preview = "", artist = new { name = "" } } }
                        }
                })?.tracks.data ?? [];

                var tracks = new List<TrackData>();
                foreach (var track in rawTrackData)
                {
                    if (!_usedTracks.Contains(track.id) && track.preview != "")
                        tracks.Add(new TrackData(track.id, track.title_short, track.artist.name, track.preview));
                }
                
                return _tracks = tracks;
            }
        }
        
        return _tracks;
    }

    private const int ResultCount = 25;
    public static async Task<TrackData> GetRandomTrack()
    {
        var tracks = await LoadTracks();
        var randomTrackIndex = Random.Shared.Next(tracks.Count);
        var randomTrack = tracks[randomTrackIndex];
        tracks.RemoveAt(randomTrackIndex);
        
        FleckLog.Info($"Random Track: {randomTrack.Artist} - {randomTrack.Name}");

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WebHit-Test", "0.0.1"));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var response = JsonConvert.DeserializeAnonymousType(await client.GetStringAsync(
                    $"https://api.discogs.com/database/search?artist={randomTrack.Artist}" +
                    $"&release_title={randomTrack.Name}&per_page={ResultCount}"), 
                new { results = new[] { new { year = "" } } });
                
            var releaseYears = response.results.ToList().ConvertAll(r =>
                r.year == null ? int.MaxValue : Convert.ToInt32(r.year));
            if (releaseYears.Count == 0)
            {
                response = JsonConvert.DeserializeAnonymousType(await client.GetStringAsync(
                        $"https://api.discogs.com/database/search?query={randomTrack.Name} - {randomTrack.Artist}"
                        + $"&type=release&per_page={ResultCount}"), 
                    new { results = new[] { new { year = "" } } });
                
                releaseYears = response.results.ToList().ConvertAll(r =>
                    r.year == null ? int.MaxValue : Convert.ToInt32(r.year));
            }
            
            releaseYears.Sort();
            
            // Falls kein Jahr emittelt werden kann, wird ein anderer Song verwendet
            if (releaseYears.Count == 0 || releaseYears[0] == int.MaxValue)
                return await GetRandomTrack();
            
            randomTrack.ReleaseYear = releaseYears[0];
        }
        catch (HttpRequestException _)
        {
            FleckLog.Error($"Error whilst getting random track ${randomTrack.Name} - {randomTrack.Artist}");
            
            throw;
        }
        
        _usedTracks.Add(randomTrack.Id);
        
        return randomTrack;
    }

    public static void ResetUsedTracks()
    {
        _usedTracks.Clear();
        _tracks.Clear();
    }
    
    public static readonly string[] Packs = [
        "14906778801", // Standard
        "14907501761", // Summer Party
        "14907501521", // Schlager Party
        "14907501981", // Guilty Pleasures
        "14907500801", // Bayern1
        "14907500701", // Soundtracks
        "14907500521", // Bingo-Pack
        "14907500301", // Christmas
        "14907500061", // Rock
        "14907499761", // Celebration
        "14907499261", // Platinum Edition
        "14907498921", // 100% US
        "14907498721", // Hip Hop
        "14907501081", // US-Pack
    ];
}