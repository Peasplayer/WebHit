using System.Net.Http.Headers;
using Fleck;
using HitsterServer.Packets;
using Newtonsoft.Json;

namespace HitsterServer.MusicData;

public class MusicManager
{
    private static DateTime _time; //Speichert wann das letzte mal Lieder abgerufen wurden
    private static List<TrackData> _tracks = new (); //Liste der Lieder
    private static List<string> _usedTracks = new ();//Liste aller benutzten Lieder
    
    public static async Task<List<TrackData>> LoadTracks()
    {
        //Lieder werden neu geladen wenn die Liste leer ist oder die Lieder älter als 15 min sind
        if (_time + TimeSpan.FromMinutes(15) < DateTime.Now || _tracks.Count == 0)
        {
            try
            {
                FleckLog.Debug("Reloading expired track data...");
                using var client = new HttpClient();
                //Lädt die Lieder aus der Deezer API
                var
                    rawApiData = //await new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("HitsterServer.json.txt")).ReadToEndAsync();
                        await client.GetStringAsync(
                            $"https://api.deezer.com/playlist/{Packs[Settings.CurrentSettings.Pack]}/");
                _time = DateTime.Now; //Speichert die Zeit wann abgefragt wurde

                //Nur die relevanten Informationen Speichern
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
                    //Lied nur hinzufügen wenn Deezer eine Vorschau hat und das lied noch nicht verwendet wurde
                    if (!_usedTracks.Contains(track.id) && track.preview != "")
                        tracks.Add(new TrackData(track.id, track.title_short.Replace(" (Mono)", ""), track.artist.name, track.preview));
                }

                return _tracks = tracks; //Die Liste neu überschrieben
            }
            catch (Exception e)
            {
                FleckLog.Error($"Error whilst loading tracks! {e.Message}\n{e.StackTrace}");
                GameServer.Instance.SendPacketEveryone(new DisconnectPacket("Server kann keine Songs abrufen!"));
            }
        }
        
        return _tracks;
    }

    private const int ResultCount = 25; //Maximale Ergebnise der Datenbank 
    
    //Zufälliges Lied aus der Liste holen
    public static async Task<TrackData> GetRandomTrack() 
    {
        var tracks = await LoadTracks();
        // Falls alle Songs verwendet wurden, werden dieselben Songs wiederverwendet
        if (tracks.Count == 0)
        {
            ResetUsedTracks();
            return await GetRandomTrack();
        }
        var randomTrackIndex = Random.Shared.Next(tracks.Count);
        var randomTrack = tracks[randomTrackIndex];
        tracks.RemoveAt(randomTrackIndex); //Lied entfernen damit das nicht wieder verwendet wird
        _usedTracks.Add(randomTrack.Id);
        
        FleckLog.Info($"Random Track: {randomTrack.Artist} - {randomTrack.Name}");

        try
        {
            //Verbindung zur Datenbank aufbauen
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WebHit-Test", "0.0.1"));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            //API Anfrage bei der nach Künstler und Titel gesucht wird
            var response = JsonConvert.DeserializeAnonymousType(await client.GetStringAsync(
                    $"https://api.discogs.com/database/search?artist={randomTrack.Artist}" +
                    $"&release_title={randomTrack.Name}&per_page={ResultCount}"), 
                new { results = new[] { new { year = "" } } });
                
            //Jahr in Zahlen umwenadeln. Wenn kein Jahr gefunden würde erhält es ein Max value damit es ganz am ende ist
            var releaseYears = response.results.ToList().ConvertAll(r =>
                r.year == null ? int.MaxValue : Convert.ToInt32(r.year));
            //Wenn nichts gefunden wurde wird hier nochmal grober gescuht
            if (releaseYears.Count == 0)
            {
                response = JsonConvert.DeserializeAnonymousType(await client.GetStringAsync(
                        $"https://api.discogs.com/database/search?query={randomTrack.Name} - {randomTrack.Artist}"
                        + $"&type=release&per_page={ResultCount}"), 
                    new { results = new[] { new { year = "" } } });
                
                releaseYears = response.results.ToList().ConvertAll(r =>
                    r.year == null ? int.MaxValue : Convert.ToInt32(r.year));
            }
            
            releaseYears.Sort(); //Aufsteigend nach Jahr sotieren
            
            // Falls kein Jahr emittelt werden kann, wird ein anderer Song verwendet
            if (releaseYears.Count == 0 || releaseYears[0] == int.MaxValue)
                return await GetRandomTrack();
            
            randomTrack.ReleaseYear = releaseYears[0];
        }
        catch (HttpRequestException e)
        {
            FleckLog.Error($"Error whilst getting random track ${randomTrack.Name} - {randomTrack.Artist}\n{e.Message}\n{e.StackTrace}");
            GameServer.Instance.SendPacketEveryone(new DisconnectPacket("Server kann keinen Song abrufen"));
        }
        
        return randomTrack;
    }

    public static void ResetUsedTracks()
    {
        _usedTracks.Clear();
        _tracks.Clear();
    }

    // Liste der Playlist-IDs der verschiedenen Packs
    private static readonly string[] Packs = [
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