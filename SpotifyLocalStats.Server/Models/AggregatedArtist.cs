using System.Runtime.InteropServices.Marshalling;
using WebApi.Models;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedArtist : AggregateBase
{
    public Artist Artist { get; set; }
    public TimeOfDayStat<AggregatedAlbum> TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?


    // these are for the user... 

    // Rethink this, do we need top tracks of the artist for the user? we just query this from the db 

    //public ICollection<Track> TopTracks { get; set; } // list instead?
    //public ICollection<Album> TopAlbums { get; set; } // list instead? aggregatedAlnbum instead?
    public int UniqueTracksPlayed { get; set; }
    public int AlbumsListened { get; set; }

}
