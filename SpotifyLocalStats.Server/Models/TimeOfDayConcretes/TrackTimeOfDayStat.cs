using SpotifyLocalStats.Server.Models;

namespace WebApi.Models.TimeOfDayConcretes
{
    public class TrackTimeOfDayStat : TimeOfDayStat<AggregatedTrack>
    {
        public TrackTimeOfDayStat(Guid aggregateId, int timeOfDay, int playCount) : base(aggregateId, timeOfDay, playCount)
        {
        }
    }
}
