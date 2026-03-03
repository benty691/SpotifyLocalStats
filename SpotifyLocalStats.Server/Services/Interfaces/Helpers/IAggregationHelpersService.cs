using SpotifyLocalStats.Server.Models;
using WebApi.Models;

namespace WebApi.Services.Interfaces.Helpers;

public interface IAggregationHelpersService<TAggregate, TTimeOfDay>
    where TAggregate : AggregateBase
    where TTimeOfDay : TimeOfDayStat<TAggregate>
{
    Task RunCalculations(Guid userId);

}
