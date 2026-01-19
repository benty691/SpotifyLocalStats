using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

//Tracks are generated via imported tracks. We do not have a catalog of all tracks.
public class Track
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Album? Album { get; set; }
    public List<Artist> Artists { get; set; }
    public string SpotifyTrackUri { get; set; } 
    public int Duration { get; set; }
    public bool IsSingle { get; set; }
    public bool IsExplicit { get; set; }
    public string SpotifyTrackId { get; set; }
    public int TrackNumber { get; set; }
    public string ReleaseDate { get; set; }
    public string ReleaseDatePrecision { get; set; } // enum later
    public int DiscNumber { get; set; }
    public string PreviewUrl { get; set; }
    public string[] AvaliableMarkets { get; set; }
    public string Href { get; set; }
    public ExternalId ExternalIds { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int TimesPlayed { get; set; } // total times played from imported data
}

public class TrackContext : DbContext
{
    public DbSet<TrackContext> Track { get; set; }
}
