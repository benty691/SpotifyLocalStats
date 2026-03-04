using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Linq.Expressions;

namespace WebApi.Services.Implementations.Helpers;

public class ArtistAggregationHelperService : AggregationHelperService<AggregatedArtist, ArtistTimeOfDayStat>
{
    public ArtistAggregationHelperService(
        ILogger<ArtistAggregationHelperService> logger,
        SpotifyStatsContext context,
        Expression<Func<ImportedTrack, string>> groupSelector,
        Func<AggregatedArtist, string> aggregateNameSelector,
        Func<ArtistTimeOfDayStat, int> timeOfDayNameSelector,
        Func<Guid, int, ArtistTimeOfDayStat> timeOfDayFactory)
        : base(logger, context, groupSelector, aggregateNameSelector, timeOfDayNameSelector, timeOfDayFactory)
    {
    }

    protected override async Task InitializeAsync(Guid userId)
    {
        _aggregates = await _context.Set<AggregatedArtist>()
            .Where(x => x.UserId == userId)
            .Include(x => x.Artist)
            .ToListAsync();

        if (!_aggregates.Any())
            throw new InvalidOperationException("No aggregates for AggregatedArtist found.");

        var existing = await _context.Set<ArtistTimeOfDayStat>()
            .Where(x => x.Aggregate.UserId == userId)
            .ToListAsync();
        _context.Set<ArtistTimeOfDayStat>().RemoveRange(existing);
        _timeOfDayStats = new List<ArtistTimeOfDayStat>();
    }

    public override async Task RunCalculations(Guid userId)
    {
        await base.RunCalculations(userId);
        await CalculateUniqueTracksListened(userId);
        await CalculateAlbumsListened(userId);
    }

    private async Task CalculateUniqueTracksListened(Guid userId)
    {
        var uniqueTrackCounts = await _context.ImportedTracks
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.MasterMetadataArtistName)
            .Select(g => new { ArtistName = g.Key, Count = g.Select(x => x.MasterMetadataTrackName).Distinct().Count() })
            .ToDictionaryAsync(x => x.ArtistName!, x => x.Count);

        foreach (var aggArtist in _aggregates)
            aggArtist.UniqueTracksPlayed = uniqueTrackCounts.GetValueOrDefault(aggArtist.Artist.Name, 0);
    }

    private async Task CalculateAlbumsListened(Guid userId)
    {
        var albumCounts = await _context.ImportedTracks
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.MasterMetadataArtistName)
            .Select(g => new { ArtistName = g.Key, Count = g.Select(x => x.MasterMetadataAlbumName).Distinct().Count() })
            .ToDictionaryAsync(x => x.ArtistName!, x => x.Count);

        foreach (var aggArtist in _aggregates)
            aggArtist.AlbumsListened = albumCounts.GetValueOrDefault(aggArtist.Artist.Name, 0);
    }
}
