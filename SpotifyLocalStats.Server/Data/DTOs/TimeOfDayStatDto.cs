using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs;

public class TimeOfDayStatDto<TAggregate> where TAggregate : AggregateBase
{
    public TAggregate Aggregate { get; set; } = null!;
    public Guid AggregateId { get; set; }
    public int TimeOfDay { get; set; }
    public int PlayCount { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
