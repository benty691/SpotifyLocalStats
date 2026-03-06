using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Linq.Expressions;
using WebApi.Models.TimeOfDayConcretes;

namespace WebApi.Services.Implementations.Helpers;

public class AlbumAggregationHelperService : AggregationHelperService<AggregatedAlbum, AlbumTimeOfDayStat>
{
    public AlbumAggregationHelperService(
        ILogger<AlbumAggregationHelperService> logger,
        SpotifyStatsContext context,
        Expression<Func<ImportedTrack, string>> groupSelector,
        Func<AggregatedAlbum, string> aggregateNameSelector,
        Func<AlbumTimeOfDayStat, int> timeOfDayNameSelector,
        Func<Guid, int, AlbumTimeOfDayStat> timeOfDayFactory)
        : base(logger, context, groupSelector, aggregateNameSelector, timeOfDayNameSelector, timeOfDayFactory)
    {
    }

    protected override async Task InitializeAsync(Guid userId)
    {
        _aggregates = await _context.Set<AggregatedAlbum>()
            .Where(x => x.UserId == userId)
            .Include(x => x.Album)
            .ToListAsync();

        if (!_aggregates.Any())
            throw new InvalidOperationException("No aggregates for AggregatedAlbum found.");

        var existing = await _context.Set<AlbumTimeOfDayStat>()
            .Where(x => x.Aggregate.UserId == userId)
            .ToListAsync();
        _context.Set<AlbumTimeOfDayStat>().RemoveRange(existing);
        _timeOfDayStats = new List<AlbumTimeOfDayStat>();
    }

    public override async Task RunCalculations(Guid userId)
    {
        await base.RunCalculations(userId);
    }
}