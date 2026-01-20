using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;
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
    public Guid Id { get; set; }
    // FK to user
    public User User { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Platform { get; set; } 
    public int MsPlayed { get; set; }
    public string ConnCountry { get; set; }
    public string MasterMetadataTrackName { get; set; }
    public string MasterMetadataArtistName { get; set; }
    public string MasterMetadataAlbumName { get; set; }
    public string SpotifyTrackUri { get; set; }
    public string EpisodeName { get; set; }
    public string EpisodeShowName { get; set; }
    public string SpotifyEpisodeUri { get; set; }
    public string AudiobookTitle { get; set; }
    public string AudiobookUri { get; set; }
    public string AudiobookChapterUri { get; set; }
    public string AudiobookChapterTitle { get; set; }
    public string ReasonStart { get; set; } // maybe enum later
    public string ReasonEnd { get; set; } // maybe enum later
    public bool IsFirstTrack { get; set; }
    public bool IsShuffle { get; set; }
    public bool IsSkipped { get; set; }
    public bool IsOffline { get; set; }
    public DateTime OfflineTimestamp { get; set; }
    public bool IncognitoMode { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ImportName { get; set; } // to identify different imports, use as PK to stop multiple of same import? 
}

