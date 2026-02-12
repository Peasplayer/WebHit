using System.Net.Http.Headers;
using System.Reflection;
using Fleck;
using Newtonsoft.Json;

namespace HitsterServer.MusicData;

public class MusicManager
{
    private static DateTime _time;
    private static JsonStructs.TrackData[] _tracks;
    private static List<string> _usedTracks = new List<string>();
    
    public static async Task<JsonStructs.TrackData[]> GetTracks()
    {
        if (_time + TimeSpan.FromMinutes(15) < DateTime.Now)
        {
            FleckLog.Debug("Reloading expired track data...");
            using (var client = new HttpClient())
            {
                var rawApiData = await new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("HitsterServer.json.txt")).ReadToEndAsync();
                //await client.GetStringAsync("https://api.deezer.com/playlist/14906778801/");
                //Console.WriteLine(rawApiData);
                _time = DateTime.Now;
                return _tracks = JsonConvert.DeserializeObject<JsonStructs.PlaylistContainer>(rawApiData).Tracks;
            }
        }
        
        return _tracks;
    }

    public static async Task<JsonStructs.TrackData> GetRandomTrack()
    {
        var tracks = await GetTracks();
        var randomTrack = tracks[Random.Shared.Next(tracks.Length)];

        while (_usedTracks.Contains(randomTrack.Id) || randomTrack.Link == "")
        {
            randomTrack = tracks[Random.Shared.Next(tracks.Length)];
        }

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WebHit.test", "0.0.1"));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var query = Uri.EscapeDataString(
                $"artistname:\"{randomTrack.Artist}\"ANDrecording:\"{randomTrack.Name}\"ANDstatus:\"official\"type:\"single\"");
            var mbRes = JsonConvert.DeserializeObject<JsonStructs.MBContainer>(await client.GetStringAsync("https://musicbrainz.org/ws/2/recording?query=" 
                + query + "&fmt=json&limit=25"));
            
            var releaseYears = mbRes.Recordings.ToList().ConvertAll(r => Convert.ToInt32(r.FirstRelease.Split("-")[0]));
            releaseYears.Sort();
            randomTrack.ReleaseYear = releaseYears[0];
        }
        
        _usedTracks.Add(randomTrack.Id);
        return randomTrack;
    }
}