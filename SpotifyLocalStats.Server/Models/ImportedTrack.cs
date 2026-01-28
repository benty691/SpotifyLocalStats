using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace SpotifyLocalStats.Server.Models;

[PrimaryKey(nameof(Id), nameof(ImportHash))]
public class ImportedTrack
{ /*
      {
        "ts": "2026-01-12T04:08:38Z",
        "platform": "windows",
        "ms_played": 197760,
        "conn_country": "AU",
        "ip_addr": "2001:8003:43d0:3800:60c9:f135:d727:43ba",
        "master_metadata_track_name": "The National Anthem",
        "master_metadata_album_artist_name": "Radiohead",
        "master_metadata_album_album_name": "Kid A",
        "spotify_track_uri": "spotify:track:4Wgj6jzoI2gYlumXdYAB8U",
        "episode_name": null,
        "episode_show_name": null,
        "spotify_episode_uri": null,
        "audiobook_title": null,
        "audiobook_uri": null,
        "audiobook_chapter_uri": null,
        "audiobook_chapter_title": null,
        "reason_start": "trackdone",
        "reason_end": "remote",
        "shuffle": false,
        "skipped": false,
        "offline": false,
        "offline_timestamp": 1768190719,
        "incognito_mode": false
      },
    */

    public ImportedTrack()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    // FK to user
    public User User { get; set; }
    [JsonProperty("ts")]
    public DateTime TimeStamp { get; set; }
    [JsonProperty("platform")]
    public string Platform { get; set; }
    [JsonProperty("ms_played")]
    public int MsPlayed { get; set; }
    [JsonProperty("conn_country")]
    public string ConnCountry { get; set; }
    [JsonProperty("master_metadata_track_name")]
    public string MasterMetadataTrackName { get; set; }
    [JsonProperty("master_metadata_album_artist_name")]
    public string MasterMetadataArtistName { get; set; }
    [JsonProperty("master_metadata_album_album_name")]
    public string MasterMetadataAlbumName { get; set; }
    [JsonProperty("spotify_track_uri")]
    public string SpotifyTrackUri { get; set; }
    [JsonProperty("episode_name")]
    public string EpisodeName { get; set; }
    [JsonProperty("episode_show_name")]
    public string EpisodeShowName { get; set; }
    [JsonProperty("spotify_episode_uri")]
    public string SpotifyEpisodeUri { get; set; }
    [JsonProperty("audiobook_title")]
    public string AudiobookTitle { get; set; }
    [JsonProperty("audiobook_uri")]
    public string AudiobookUri { get; set; }
    [JsonProperty("audiobook_chapter_uri")]
    public string AudiobookChapterUri { get; set; }
    [JsonProperty("audiobook_chapter_title")]
    public string AudiobookChapterTitle { get; set; }
    [JsonProperty("reason_start")]
    public string ReasonStart { get; set; } // maybe enum later
    [JsonProperty("reason_end")]
    public string ReasonEnd { get; set; } // maybe enum later
    [JsonProperty("shuffle")]
    public bool IsShuffle { get; set; }
    [JsonProperty("skipped")]
    public bool IsSkipped { get; set; }
    [JsonProperty("offline")]
    public bool IsOffline { get; set; }
    [JsonProperty("offline_timestamp")]
    public DateTime OfflineTimestamp { get; set; }
    [JsonProperty("incognito_mode")]
    public bool IncognitoMode { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ImportHash { get; set; } // hash, we check has this exact item been uploaded before? This is a part of the PK of the tbale
}

