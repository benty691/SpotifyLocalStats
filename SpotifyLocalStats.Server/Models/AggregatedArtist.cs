using System.Runtime.InteropServices.Marshalling;
using WebApi.Models;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedArtist : AggregateBase
{

    public AggregatedArtist(Artist artist)
    {
        TimeOfDayStats = new List<TimeOfDayStat<AggregatedArtist>>();
        Artist = artist;
    }
    public Artist Artist { get; set; }
    public ICollection<TimeOfDayStat<AggregatedArtist>> TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
    public int UniqueTracksPlayed { get; set; }
    public int AlbumsListened { get; set; }

}
