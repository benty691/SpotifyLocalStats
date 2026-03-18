using SpotifyLocalStats.Api;

namespace SpotifyLocalStats.Server.Models;

public class Album : BaseModel
{
    public Album()
    {
        Images = new List<Image>();
        Tracks = new List<Track>();
        ExternalIds = new List<ExternalId>();
        Artists = new List<Artist>();
    }
    public Album(string name, List<Artist> artist)
    {
        Artists = artist;
        Name = name;
    }


    public string Name { get; set; }
    public string? Type { get; set; } // album, single, compilation
    public int? TotalTracks { get; set; }
    public List<Artist> Artists { get; set; }
    public string[]? AvaliableMarkets { get; set; }
    public string? Href { get; set; }
    public string? SpotifyId { get; set; }
    public string? SpotifyUrl { get; set; }
    public ICollection<Image> Images { get; set; }
    public string? RealeaseDate { get; set; }
    public AlbumBaseRelease_date_precision? ReleaseDatePrecision { get; set; } // year, month, day
    public ICollection<Track> Tracks { get; set; }
    public Guid? CopyrightId { get; set; }
    public CopyrightContent? Copyright { get; set; }
    public ICollection<ExternalId> ExternalIds { get; set; }
    public string? Label { get; set; }
    public int? TimesPlayed { get; set; } // total times played from tracks 'TimePlayed' field..., i.e how many times has a track from this album been played?? not sure
}

