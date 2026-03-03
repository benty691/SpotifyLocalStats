using WebApi.Models;
using WebApi.Models.TimeOfDayConcretes;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack : AggregateBase
{
    public AggregatedTrack()
    {
        TimeOfDayStats = new List<TrackTimeOfDayStat>();
    }
    public AggregatedTrack(Track track) : this()
    {
        Track = track;
    }

    public Track Track { get; set; } = null!;
    public ICollection<TrackTimeOfDayStat> TimeOfDayStats { get; set; }
}
