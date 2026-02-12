using WebApi.Models;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack : AggregateBase 
{
    public AggregatedTrack()
    {
        TimeOfDayStats = new List<TimeOfDayStat<AggregatedTrack>>();
    }
    public Track Track { get; set; }
    public ICollection<TimeOfDayStat<AggregatedTrack>> TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
}
