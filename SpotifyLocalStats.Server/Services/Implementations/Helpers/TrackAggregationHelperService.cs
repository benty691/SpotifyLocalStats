using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Linq.Expressions;
using WebApi.Models.TimeOfDayConcretes;

namespace WebApi.Services.Implementations.Helpers;

public class TrackAggregationHelperService : AggregationHelperService<AggregatedTrack, TrackTimeOfDayStat>
{
    public TrackAggregationHelperService(
        ILogger<TrackAggregationHelperService> logger,
        SpotifyStatsContext context,
        Expression<Func<ImportedTrack, string>> groupSelector,
        Func<AggregatedTrack, string> aggregateNameSelector,
        Func<TrackTimeOfDayStat, int> timeOfDayNameSelector,
        Func<Guid, int, TrackTimeOfDayStat> timeOfDayFactory)
        : base(logger, context, groupSelector, aggregateNameSelector, timeOfDayNameSelector, timeOfDayFactory)
    {
    }

    protected override async Task InitializeAsync(Guid userId)
    {
        _aggregates = await _context.Set<AggregatedTrack>()
            .Where(x => x.UserId == userId)
            .Include(x => x.Track)
            .ToListAsync();

        if (!_aggregates.Any())
            throw new InvalidOperationException("No aggregates for AggregatedTrack found.");

        var existing = await _context.Set<TrackTimeOfDayStat>()
            .Where(x => x.Aggregate.UserId == userId)
            .ToListAsync();
        _context.Set<TrackTimeOfDayStat>().RemoveRange(existing);
        _timeOfDayStats = new List<TrackTimeOfDayStat>();
    }

    public override async Task RunCalculations(Guid userId)
    {
        await base.RunCalculations(userId);
    }
}