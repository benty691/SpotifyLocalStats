using WebApi.Models;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack : AggregateBase 
{
    public Track Track { get; set; }
    public TimeOfDayStat<AggregatedAlbum> TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
}
