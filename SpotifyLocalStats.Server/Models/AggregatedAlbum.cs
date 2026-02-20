using System.Security.Cryptography.Xml;
using WebApi.Models;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedAlbum : AggregateBase
{
    public AggregatedAlbum()
    {
        TimeOfDayStats = new List<TimeOfDayStat<AggregatedAlbum>>();
    }
    public AggregatedAlbum(Album album) : this()
    {
        Album = album;
    }
    public Album Album { get; set; }
    public ICollection<TimeOfDayStat<AggregatedAlbum>> TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
    public int TimesCompleted { get; set; } // ehhh really hard to figure out? have to model album, figure out how many times played through, no shufffle, no skipping,???
}
