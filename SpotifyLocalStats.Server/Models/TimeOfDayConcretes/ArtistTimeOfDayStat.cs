using SpotifyLocalStats.Server.Models;
using WebApi.Models;

public class ArtistTimeOfDayStat : TimeOfDayStat<AggregatedArtist>
{
    public ArtistTimeOfDayStat(Guid aggregateId, int timeOfDay, int playCount) : base(aggregateId, timeOfDay, playCount)
    {
    }
}

