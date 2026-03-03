using SpotifyLocalStats.Server.Models;

namespace WebApi.Models.TimeOfDayConcretes;

public class AlbumTimeOfDayStat : TimeOfDayStat<AggregatedAlbum>
{
    public AlbumTimeOfDayStat(Guid aggregateId, int timeOfDay, int playCount) : base(aggregateId, timeOfDay, playCount)
    {
    }
}
