using System.Runtime.InteropServices.Marshalling;
using WebApi.Models;
using WebApi.Models.TimeOfDayConcretes;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedArtist : AggregateBase
{
    public AggregatedArtist()
    {
        TimeOfDayStats = new List<ArtistTimeOfDayStat>();
    }
    public AggregatedArtist(Artist artist) : this()
    {
        Artist = artist;
    }

    public Artist Artist { get; set; }
    public ICollection<ArtistTimeOfDayStat> TimeOfDayStats { get; set; }
    public int UniqueTracksPlayed { get; set; }
    public int AlbumsListened { get; set; }

}
