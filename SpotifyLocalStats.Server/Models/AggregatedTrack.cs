namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack : AggregateBase 
{
    public Track Track { get; set; }
    public Guid TrackId { get; set; }
    public ICollection<Artist> Artists { get; set; }
    public ICollection<Album> Albums { get; set; } // can bo on multiple albums, or non (Single)
}
