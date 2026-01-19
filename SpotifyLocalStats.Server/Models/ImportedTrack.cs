using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

public class ImportedTrack
{
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

public class ImportedTrackContext : DbContext
{
    public DbSet<ImportedTrack> ImportedTracks { get; set; }
}
