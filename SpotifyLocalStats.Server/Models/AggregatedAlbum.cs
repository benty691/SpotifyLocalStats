using System.Security.Cryptography.Xml;
using WebApi.Models;
using WebApi.Models.TimeOfDayConcretes;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedAlbum : AggregateBase
{
    public AggregatedAlbum()
    {
        TimeOfDayStats = new List<AlbumTimeOfDayStat>();
    }
    public AggregatedAlbum(Album album) : this()
    {
        Album = album;
    }
    public Album Album { get; set; }
    public ICollection<AlbumTimeOfDayStat> TimeOfDayStats { get; set; }
    public int TimesCompleted { get; set; } // ehhh really hard to figure out? have to model album, figure out how many times played through, no shufffle, no skipping,???
}
