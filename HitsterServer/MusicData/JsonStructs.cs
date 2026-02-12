using Newtonsoft.Json;

namespace HitsterServer.MusicData;

public class JsonStructs
{
    public struct TrackData
    {
        [JsonProperty("id")]
        public string Id;
        
        [JsonProperty("title_short")]
        public string Name;
    
        [JsonProperty("artist")]
        private ArtistContainer _artist;

        [JsonIgnore]
        public String Artist => _artist.Name;
    
        [JsonProperty("preview")]
        public string Link;

        [JsonIgnore]
        public int ReleaseYear;
    }

    public struct ArtistContainer
    {
        [JsonProperty("name")]
        public string Name;
    }

    public struct TrackList
    {
        [JsonProperty("data")]
        public TrackData[] Tracks;
    }
    
    public struct PlaylistContainer
    {
        [JsonProperty("tracks")]
        private TrackList _list;
        
        [JsonIgnore]
        public TrackData[] Tracks => _list.Tracks;
    }

    public struct MBContainer
    {
        [JsonProperty("recordings")]
        public MBRecording[] Recordings;
    }

    public struct MBRecording
    {
        [JsonProperty("first-release-date")]
        public string FirstRelease;
    }
}