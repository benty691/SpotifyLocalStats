using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

//Tracks are generated via imported tracks. We do not have a catalog of all tracks.
public class Track : BaseModel
{
    public Track(Artist artist, Album album , string name ,string spotifyTrackUri)
    {
        ExternalIds = new List<ExternalId>();
        Name = name;
        Album = album;
        Artist = artist;
        ExternalIds = new List<ExternalId>();
        SpotifyTrackUri = spotifyTrackUri ?? string.Empty;
    }

    public string Name { get; set; }
    public Album Album { get; set; } // can in theory be on multiple albums 
    public Artist Artist { get; set; }
    public string? SpotifyTrackUri { get; set; } = string.Empty;
    public int? MsPlayed { get; set; }
    public bool? IsSingle { get; set; }
    public bool? IsExplicit { get; set; }
    public string? SpotifyTrackId { get; set; }
    public int? TrackNumber { get; set; }
    public string? ReleaseDate { get; set; }
    public string? ReleaseDatePrecision { get; set; } // enum later
    public int? DiscNumber { get; set; }
    public string? PreviewUrl { get; set; }
    public string[]? AvaliableMarkets { get; set; }
    public string? Href { get; set; }
    public ICollection<ExternalId> ExternalIds { get; set; }
    public int? TimesPlayed { get; set; } // This should be either calculated from aggregates. or a seperate table called trackStats... 
}
