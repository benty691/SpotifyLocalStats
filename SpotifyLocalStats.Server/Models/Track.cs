using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

//Tracks are generated via imported tracks. We do not have a catalog of all tracks.
public class Track : BaseModel
{
    public Track()
    {
        Artists = new List<Artist>();
        Album = new List<Album>();
        ExternalIds = new List<ExternalId>();
    }

    public string Name { get; set; }
    public ICollection<Album> Album { get; set; } // can in theory be on multiple albums 
    public ICollection<Artist> Artists { get; set; }
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
    public ICollection<ExternalId> ExternalIds { get; set; }
    public int TimesPlayed { get; set; } // total times played from imported data (gloabl total of imported data)
}
