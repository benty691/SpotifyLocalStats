namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack
{
    public Guid Id { get; set; }
    public User User { get; }
    public Track Track { get; }
    public Artist Artist { get; }
    public Album Album { get; }
    public int PlayCount { get; set; }
    public int msListend { get; set; }
    public int minsListend { get; set; } // set from msListend
}
