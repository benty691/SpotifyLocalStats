using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Models;
using WebApi.Models.TimeOfDayConcretes;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;


// I want to mkae the type of this class (album, track, artits) a generic sop we consolidate three classes into one.. 
public sealed class AlbumAggregationHelperService : IAlbumAggregationHelpersService
{
    private readonly ILogger<AlbumAggregationHelperService> _logger;
    private readonly SpotifyStatsContext _context;
    private List<AggregatedAlbum> _aggregatedAlbums = new List<AggregatedAlbum>();

    public AlbumAggregationHelperService(ILogger<AlbumAggregationHelperService> logger, SpotifyStatsContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitializeAggregatedArtistsAsync()
    {
        _aggregatedAlbums = await GetAggregatedAlbums();
    }

    private async Task<List<AggregatedAlbum>> GetAggregatedAlbums()
    {
        var aggAlbums = await _context.AggregatedAlbums.ToListAsync();

        if (!aggAlbums.Any())
        {
            throw new InvalidOperationException("No aggregated Albums found.");
        }
        return aggAlbums;
    }

    public async Task RunCalculations(Guid userId)
    {
        await InitializeAggregatedArtistsAsync();
        await CalculateLongestStreak(userId);
        await CalculateDrySpell(userId);
        await CalculateMostTimesIn24Hours(userId);
        await CalculateTopListeningDate(userId);
        await TimeOfDayStats(userId);
    }

    private async Task CalculateTopListeningDate(Guid userId)
    {
        var albumGroups = await _context.ImportedTracks
            .Where(x => x.User.Id == userId)
            .GroupBy(x => x.MasterMetadataAlbumName)
            .ToListAsync();

        var aggAlbumDict = _aggregatedAlbums
           .ToDictionary(x => x.Album.Name, x => x);

        foreach (var albumGroup in albumGroups)
        {
            if (!aggAlbumDict.TryGetValue(albumGroup.Key!, out var aggregatedAlbum))
                continue;

            var topDay = albumGroup
                .GroupBy(x => x.TimeStamp.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    PlayCount = g.Count()
                })
                .OrderByDescending(g => g.PlayCount)
                .FirstOrDefault();

            if (topDay == null)
                throw new ArgumentNullException($"{topDay} is null");

            aggregatedAlbum.TopListeningDate = topDay.Date;
        }
    }

    private async Task CalculateMostTimesIn24Hours(Guid userId)
    {
        var playsIn24Hours = 1;

        var albumGroups = await _context.ImportedTracks
                 .Where(x => x.UserId == userId)
                 .GroupBy(x => x.MasterMetadataAlbumName)
                 .ToListAsync();

        foreach (var albumGroup in albumGroups)
        {
            var orderedAlbums = albumGroup
                .Select(x => x.TimeStamp)
                .OrderBy(x => x)
                .ToList();

            var aggAlbumDict = _aggregatedAlbums
            .ToDictionary(x => x.Album.Name, x => x);

            foreach (var track in albumGroup)
            {
                if (!aggAlbumDict.TryGetValue(albumGroup.Key!, out var aggregatedAlbum))
                    continue;

                var trackTime = track.TimeStamp;
                var startTime = trackTime.AddHours(-24);
                var nextPlaysIn24Hours = albumGroup
                    .Where(x => x.TimeStamp >= startTime && x.TimeStamp < trackTime)
                    .Count();

                if (nextPlaysIn24Hours > playsIn24Hours)
                {
                    playsIn24Hours = nextPlaysIn24Hours;
                }

                aggregatedAlbum.MostTimesIn24Hours = playsIn24Hours;
            }
        }
    }

    private async Task TimeOfDayStats(Guid userId)
    {
        var albumGroups = await _context.ImportedTracks
                .Where(x => x.UserId == userId)
                .GroupBy(x => x.MasterMetadataAlbumName)
                .ToListAsync();

        var aggAlbumDict = _aggregatedAlbums
            .ToDictionary(x => x.Album.Name, x => x);

        var timeOfDayStatsForUser = _context.AlbumTimeOfDayStats
            .Where(x => x.Aggregate.UserId == userId)
            .ToDictionary(x => x.TimeOfDay);

        foreach (var albumGroup in albumGroups)
        {
            var timeOfDays = albumGroup
                .Select(x => x.TimeStamp)
                .ToList();

            if (!aggAlbumDict.TryGetValue(albumGroup.Key!, out var aggregatedAlbum))
                continue;

            foreach (var timeOfDay in timeOfDays)
            {
                if (!timeOfDayStatsForUser.TryGetValue(timeOfDay.Hour, out var timeOfDayStat)) //i.e this time of day for this album doenst exist yet
                {
                    var newTimeOfDay = new AlbumTimeOfDayStat(aggregatedAlbum.Id, timeOfDay.Hour, 1)
                    {
                        Aggregate = aggregatedAlbum,
                    };
                    await _context.AlbumTimeOfDayStats.AddAsync(newTimeOfDay);
                    timeOfDayStatsForUser[timeOfDay.Hour] = newTimeOfDay;
                }
                else
                {
                    timeOfDayStat.PlayCount += 1;
                    timeOfDayStat.LastUpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    private async Task CalculateLongestStreak(Guid userId)
    {
        var albumGroups = await _context.ImportedTracks
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.MasterMetadataAlbumName)
            .ToListAsync();

        var aggAlbumDict = _aggregatedAlbums
            .ToDictionary(x => x.Album.Name, x => x);

        foreach (var albumGroup in albumGroups)
        {
            if (!aggAlbumDict.TryGetValue(albumGroup.Key!, out var aggregatedAlbum))
                continue;

            var orderedDates = albumGroup
                .Select(x => x.TimeStamp.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (!orderedDates.Any())
                continue;

            int longestStreak = 1;
            int currentStreak = 1;

            DateTime longestStreakEndDate = orderedDates[0];
            DateTime previousDate = orderedDates[0];

            for (int i = 1; i < orderedDates.Count; i++)
            {
                var currentDate = orderedDates[i];

                if (currentDate == previousDate.AddDays(1))
                {
                    currentStreak++;

                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                        longestStreakEndDate = currentDate;
                    }
                }
                else
                {
                    currentStreak = 1;
                }

                previousDate = currentDate;
            }

            aggregatedAlbum.LongestStreakDays = longestStreak;
            aggregatedAlbum.LongestStreakEndDate = longestStreakEndDate;
            aggregatedAlbum.LongestStreakStartDate = longestStreakEndDate.AddDays(-(longestStreak - 1));
        }
    }

    private async Task CalculateDrySpell(Guid userId)
    {
        var albumGroups = await _context.ImportedTracks
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.MasterMetadataAlbumName)
            .ToListAsync();

        var aggAlbumDict = _aggregatedAlbums
            .ToDictionary(x => x.Album.Name, x => x);

        foreach (var albumGroup in albumGroups)
        {
            var drySpellStartDate = new DateTime();
            var dryStreakEndDate = new DateTime();
            var drySpell = 0;

            if (!aggAlbumDict.TryGetValue(albumGroup.Key, out var aggregatedAlbum))
                continue;

            var orderedDates = albumGroup
                .Select(x => x.TimeStamp.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            for (var i = 0; i < orderedDates.Count; i++)
            {
                if (i == 0)
                {
                    drySpellStartDate = orderedDates[i].Date;
                    dryStreakEndDate = orderedDates[i].Date;
                    continue;
                }

                if (drySpell < (orderedDates[i].Date - orderedDates[i - 1].Date).Days)
                {
                    drySpell = (orderedDates[i].Date - orderedDates[i - 1].Date).Days;
                    drySpellStartDate = orderedDates[i - 1].Date;
                    dryStreakEndDate = orderedDates[i].Date;
                }
            }
            aggregatedAlbum.LongestDrySpellEnd = dryStreakEndDate;
            aggregatedAlbum.LongestDrySpellStart = drySpellStartDate;
            aggregatedAlbum.LongestDrySpell = drySpell;
        }
    }

    // this is impossible right now. we need to get spotfy data :(
    // thinking define order in a array
    // iterate through tracks, tracks[i] == array[i]
    // if yes count++, else no 
    // may actually be better to create a linked list where next just points to next track and so
    /*
    public Task CalculateTimesListendToInOrder()
    {

    }
    */
}
