using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SpotifyLocalStats.Server.Models;

[Index(nameof(TimeStamp), nameof(SpotifyTrackUri), nameof(UserId), IsUnique = true, Name = "IX_TsAndSpotifyUriAndUserId")]
public class ImportedTrack
{ /*
      {
        "ts": "2026-01-12T04:08:38Z",
        "platform": "windows",
        "ms_played": 197760,
        "conn_country": "AU",
        "ip_addr": xxxxxxx,
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

    [Key]
    public Guid Id { get; set; }
    // FK to user
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
    [JsonPropertyName("ts")]
    public required DateTime TimeStamp { get; set; } // thbis is actually a string in the json, but we need to convert to dt somehow? 
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }
    [JsonPropertyName("ms_played")]
    public int? MsPlayed { get; set; }
    [JsonPropertyName("conn_country")]
    public string? ConnCountry { get; set; }
    [JsonPropertyName("master_metadata_track_name")]
    public string? MasterMetadataTrackName { get; set; }
    [JsonPropertyName("master_metadata_album_artist_name")]
    public string? MasterMetadataArtistName { get; set; }
    [JsonPropertyName("master_metadata_album_album_name")]
    public string? MasterMetadataAlbumName { get; set; }
    [JsonPropertyName("spotify_track_uri")]
    public string? SpotifyTrackUri { get; set; }
    [JsonPropertyName("episode_name")]
    public string? EpisodeName { get; set; }
    [JsonPropertyName("episode_show_name")]
    public string? EpisodeShowName { get; set; }
    [JsonPropertyName("spotify_episode_uri")]
    public string? SpotifyEpisodeUri { get; set; }
    [JsonPropertyName("audiobook_title")]
    public string? AudiobookTitle { get; set; }
    [JsonPropertyName("audiobook_uri")]
    public string? AudiobookUri { get; set; }
    [JsonPropertyName("audiobook_chapter_uri")]
    public string? AudiobookChapterUri { get; set; }
    [JsonPropertyName("audiobook_chapter_title")]
    public string? AudiobookChapterTitle { get; set; }
    [JsonPropertyName("reason_start")]
    public string? ReasonStart { get; set; } // maybe enum later
    [JsonPropertyName("reason_end")]
    public string? ReasonEnd { get; set; } // maybe enum later
    [JsonPropertyName("shuffle")]
    public bool? IsShuffle { get; set; }
    [JsonPropertyName("skipped")]
    public bool? IsSkipped { get; set; }
    [JsonPropertyName("offline")]
    public bool? IsOffline { get; set; }
    [JsonPropertyName("offline_timestamp")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? OfflineTimestamp { get; set; }
    [JsonPropertyName("incognito_mode")]
    public bool? IncognitoMode { get; set; }
    public DateTime CreatedAt { get; set; }
}

