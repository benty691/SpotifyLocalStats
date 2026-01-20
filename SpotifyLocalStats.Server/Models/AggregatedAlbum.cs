namespace SpotifyLocalStats.Server.Models;

public class AggregatedAlbum : AggregateBase
{
    public Album Album { get; set; }
    public ICollection<Artist> Artists { get; set; } 
    public ICollection<Track> TopTracks { get; set; } // tracks from album 
    public int TimesCompleted { get; set; } // ehhh really hard to figure out? have to model album, figure out how many times played through, no shufffle, no skipping,???
}
