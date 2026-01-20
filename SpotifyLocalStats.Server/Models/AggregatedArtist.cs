using System.Runtime.InteropServices.Marshalling;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedArtist : AggregateBase
{
    public Artist Artist { get; set; }

    // these are for the user... 
    public ICollection<Track> TopTracks { get; set; } // list instead?
    public ICollection<Album> TopAlbums { get; set; } // list instead? aggregatedAlnbum instead?
    public int UniqueTracksPlayed { get; set; }
    public int AlbumsListened { get; set; }

}
