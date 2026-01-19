using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SpotifyLocalStats.Server.Models;

public class Album
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // album, single, compilation
    public int TotalTracks { get; set; }
    public Artist? Artist { get; set; }
    public string[] AvaliableMarkets { get; set; }
    public string Href { get; set; }
    public string SpotifyId { get; set; }
    public string SpotifyUrl { get; set; }
    public List<Image> Images { get; set; }
    public string RealeaseDate { get; set; }
    public string ReleaseDatePrecision { get; set; } // year, month, day
    public List<Track> Tracks { get; set; }
    public CopyrightContent Copyright { get; set; }
    public List<ExternalId> ExternalIds { get; set; }
    public string Label { get; set; }
    public int TimesPlayed { get; set; } // total times played from tracks 'TimePlayed' field..., i.e how many times has a track from this album been played?? not sure
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AlbumContext : DbContext
{
    public DbSet<Album> Albums { get; set; }
}
