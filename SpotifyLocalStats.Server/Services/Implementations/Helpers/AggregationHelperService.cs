using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Linq.Expressions;
using WebApi.Models;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;


// I want to mkae the type of this class (album, track, artits) a generic sop we consolidate three classes into one.. 
public sealed class AggregationHelperService<TAggregate, TTimeOfDay> : IAggregationHelpersService<TAggregate, TTimeOfDay>
    where TAggregate : AggregateBase
    where TTimeOfDay : TimeOfDayStat<TAggregate>
{
    private readonly ILogger<AggregationHelperService<TAggregate, TTimeOfDay>> _logger;
    private readonly SpotifyStatsContext _context;
    private Func<TAggregate, string> _aggregateNameSelector;
    private Func<TTimeOfDay, int> _timeofDayNameSelector;
    private Expression<Func<ImportedTrack, string>> _groupSelector;
    private Func<Guid, int, TTimeOfDay> _timeOfDayFactory;


    private List<TAggregate> _aggregates = new List<TAggregate>();
    private List<TTimeOfDay> _timeOfDayStats = new List<TTimeOfDay>();

    public AggregationHelperService(
        ILogger<AggregationHelperService<TAggregate,
        TTimeOfDay>> logger, SpotifyStatsContext context,
        Expression<Func<ImportedTrack, string>> groupSelector,
        Func<TAggregate, string> aggregateNameSelector,
        Func<TTimeOfDay, int> timeOfDayNameSelector,
        Func<Guid, int, TTimeOfDay> timeOfDayFactory
        )
    {
        _logger = logger;
        _context = context;
        _groupSelector = groupSelector;
        _aggregateNameSelector = aggregateNameSelector;
        _timeofDayNameSelector = timeOfDayNameSelector;
        _timeOfDayFactory = timeOfDayFactory;
    }

    private async Task InitializeAsync()
    {
        _aggregates = await _context.Set<TAggregate>().ToListAsync(); // will have to look into this 
        _timeOfDayStats = await _context.Set<TTimeOfDay>().ToListAsync();

        if (!_aggregates.Any())
        {
            throw new InvalidOperationException($"No aggregates for type {typeof(TAggregate)} found.");
        }
    }

    public async Task RunCalculations(Guid userId)
    {
        var modelGroups = await _context.ImportedTracks
            .Where(x => x.UserId == userId)
            .GroupBy(_groupSelector)
            .ToListAsync();

        var aggregateDict = _aggregates
            .ToDictionary(x => _aggregateNameSelector(x), x => x);

        var timeOfDayStatsForUser = _timeOfDayStats
            .ToDictionary(x => _timeofDayNameSelector(x), x => x);

        await InitializeAsync();
        CalculateLongestStreak(userId, modelGroups, aggregateDict);
        CalculateDrySpell(userId, modelGroups, aggregateDict);
        await CalculateMostTimesIn24Hours(userId, modelGroups, aggregateDict);
        CalculateTopListeningDate(userId, modelGroups, aggregateDict);
        TimeOfDayStats(userId, modelGroups, aggregateDict, timeOfDayStatsForUser);
    }

    private void CalculateTopListeningDate(Guid userId, List<IGrouping<string, ImportedTrack>> modelGroups, Dictionary<string, TAggregate> aggregateDict)
    {
        foreach (var modelGroup in modelGroups)
        {
            if (!aggregateDict.TryGetValue(modelGroup.Key!, out var aggregate))
                continue;

            var topDay = modelGroup
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

            aggregate.TopListeningDate = topDay.Date;
        }
    }

    private async Task CalculateMostTimesIn24Hours(Guid userId, List<IGrouping<string, ImportedTrack>> modelGroups, Dictionary<string, TAggregate> aggregateDict)
    {
        var playsIn24Hours = 1;

        foreach (var modelGroup in modelGroups)
        {
            var orderedAlbums = modelGroup
                .Select(x => x.TimeStamp)
                .OrderBy(x => x)
                .ToList();

            foreach (var track in modelGroup)
            {
                if (!aggregateDict.TryGetValue(modelGroup.Key!, out var aggregate))
                    continue;

                var trackTime = track.TimeStamp;
                var startTime = trackTime.AddHours(-24);
                var nextPlaysIn24Hours = modelGroup
                    .Where(x => x.TimeStamp >= startTime && x.TimeStamp < trackTime)
                    .Count();

                if (nextPlaysIn24Hours > playsIn24Hours)
                {
                    playsIn24Hours = nextPlaysIn24Hours;
                }

                aggregate.MostTimesIn24Hours = playsIn24Hours;
            }
        }
    }

    private void TimeOfDayStats(Guid userId, List<IGrouping<string, ImportedTrack>> modelGroups, Dictionary<string, TAggregate> aggregateDict, Dictionary<int, TTimeOfDay> timeOfDayDict)
    {
        // issue is here. need geenric 

        foreach (var modelGroup in modelGroups)
        {
            var timeOfDays = modelGroup
                .Select(x => x.TimeStamp)
                .ToList();

            if (!aggregateDict.TryGetValue(modelGroup.Key!, out var aggregate))
                continue;

            foreach (var timeOfDay in timeOfDays)
            {
                if (!timeOfDayDict.TryGetValue(timeOfDay.Hour, out var timeOfDayStat)) //i.e this time of day for this album doenst exist yet
                {
                    var newTimeOfDay = _timeOfDayFactory(aggregate.Id, timeOfDay.Hour); // <-- use factory
                    newTimeOfDay.Aggregate = aggregate;
                    _timeOfDayStats.Add(newTimeOfDay);
                    timeOfDayDict[timeOfDay.Hour] = newTimeOfDay;
                }
                else
                {
                    timeOfDayStat.PlayCount += 1;
                    timeOfDayStat.LastUpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    private void CalculateLongestStreak(Guid userId, List<IGrouping<string, ImportedTrack>> modelGroups, Dictionary<string, TAggregate> aggregateDict)
    {
        foreach (var modelGroup in modelGroups)
        {
            if (!aggregateDict.TryGetValue(modelGroup.Key!, out var aggregate))
                continue;

            var orderedDates = modelGroup
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

            aggregate.LongestStreakDays = longestStreak;
            aggregate.LongestStreakEndDate = longestStreakEndDate;
            aggregate.LongestStreakStartDate = longestStreakEndDate.AddDays(-(longestStreak - 1));
        }
    }

    private void CalculateDrySpell(Guid userId, List<IGrouping<string, ImportedTrack>> modelGroups, Dictionary<string, TAggregate> aggregateDict)
    {
        foreach (var modelGroup in modelGroups)
        {
            var drySpellStartDate = new DateTime();
            var dryStreakEndDate = new DateTime();
            var drySpell = 0;

            if (!aggregateDict.TryGetValue(modelGroup.Key, out var aggregate))
                continue;

            var orderedDates = modelGroup
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
            aggregate.LongestDrySpellEnd = dryStreakEndDate;
            aggregate.LongestDrySpellStart = drySpellStartDate;
            aggregate.LongestDrySpell = drySpell;
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
