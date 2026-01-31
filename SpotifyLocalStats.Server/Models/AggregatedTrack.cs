namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack : AggregateBase 
{
    public Track Track { get; set; }
    public Guid TrackId { get; set; }
}
