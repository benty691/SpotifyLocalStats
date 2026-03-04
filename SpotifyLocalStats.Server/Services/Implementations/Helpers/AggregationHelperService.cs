using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Linq.Expressions;
using WebApi.Models;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;


// I want to mkae the type of this class (album, track, artits) a generic sop we consolidate three classes into one.. 
public class AggregationHelperService<TAggregate, TTimeOfDay> : IAggregationHelpersService<TAggregate, TTimeOfDay>
    where TAggregate : AggregateBase
    where TTimeOfDay : TimeOfDayStat<TAggregate>
{
    protected ILogger _logger;
    protected readonly SpotifyStatsContext _context;
    private Func<TAggregate, string> _aggregateNameSelector;
    private Func<TTimeOfDay, int> _timeofDayNameSelector;
    private Expression<Func<ImportedTrack, string>> _groupSelector;
    private Func<Guid, int, TTimeOfDay> _timeOfDayFactory;

    protected List<TAggregate> _aggregates = new List<TAggregate>();
    protected List<TTimeOfDay> _timeOfDayStats = new List<TTimeOfDay>();

    public AggregationHelperService(
        ILogger logger,
        SpotifyStatsContext context,
        Expression<Func<ImportedTrack, string>> groupSelector,
        Func<TAggregate, string> aggregateNameSelector,
        Func<TTimeOfDay, int> timeOfDayNameSelector,
        Func<Guid, int, TTimeOfDay> timeOfDayFactory)
    {
        _logger = logger;
        _context = context;
        _groupSelector = groupSelector;
        _aggregateNameSelector = aggregateNameSelector;
        _timeofDayNameSelector = timeOfDayNameSelector;
        _timeOfDayFactory = timeOfDayFactory;
    }

    protected virtual async Task InitializeAsync(Guid userId)
    {
        _aggregates = await _context.Set<TAggregate>().Where(x => x.UserId == userId).ToListAsync();

        if (!_aggregates.Any())
            throw new InvalidOperationException($"No aggregates for type {typeof(TAggregate)} found.");

        var existing = await _context.Set<TTimeOfDay>().Where(x => x.Aggregate.UserId == userId).ToListAsync();
        _context.Set<TTimeOfDay>().RemoveRange(existing);
        _timeOfDayStats = new List<TTimeOfDay>();
    }

    public virtual async Task RunCalculations(Guid userId)
    {
        await InitializeAsync(userId);

        var modelGroups = await _context.ImportedTracks
            .Where(x => x.UserId == userId)
            .GroupBy(_groupSelector)
            .ToListAsync();

        var aggregateDict = _aggregates
            .ToDictionary(x => _aggregateNameSelector(x), x => x);

        CalculateLongestStreak(userId, modelGroups, aggregateDict);
        CalculateDrySpell(userId, modelGroups, aggregateDict);
        await CalculateMostTimesIn24Hours(userId, modelGroups, aggregateDict);
        CalculateTopListeningDate(userId, modelGroups, aggregateDict);
        TimeOfDayStats(userId, modelGroups, aggregateDict);
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
        foreach (var modelGroup in modelGroups)
        {
            if (!aggregateDict.TryGetValue(modelGroup.Key!, out var aggregate))
                continue;

            var timestamps = modelGroup
                .Select(x => x.TimeStamp)
                .OrderBy(x => x)
                .ToList();

            var left = 0;
            var maxPlays = 1;
            var window = TimeSpan.FromHours(24);

            for (var right = 0; right < timestamps.Count; right++)
            {
                while (timestamps[right] - timestamps[left] >= window)
                    left++;

                var windowSize = right - left + 1;
                if (windowSize > maxPlays)
                    maxPlays = windowSize;
            }

            aggregate.MostTimesIn24Hours = maxPlays;
        }
    }

    private void TimeOfDayStats(Guid userId, List<IGrouping<string, ImportedTrack>> modelGroups, Dictionary<string, TAggregate> aggregateDict)
    {
        var timeOfDayDict = _timeOfDayStats.ToDictionary(x => (x.TimeOfDay, x.Aggregate.Id), x => x);

        foreach (var modelGroup in modelGroups)
        {
            if (!aggregateDict.TryGetValue(modelGroup.Key!, out var aggregate))
                continue;

            var countPerHour = modelGroup
                .GroupBy(x => x.TimeStamp.Hour)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (hour, count) in countPerHour)
            {
                if (!timeOfDayDict.TryGetValue((hour, aggregate.Id), out var timeOfDayStat))
                {
                    var newTimeOfDay = _timeOfDayFactory(aggregate.Id, hour);
                    newTimeOfDay.PlayCount = count;
                    newTimeOfDay.Aggregate = aggregate;
                    _context.Set<TTimeOfDay>().Add(newTimeOfDay);
                    timeOfDayDict[(hour, aggregate.Id)] = newTimeOfDay;
                }
                else
                {
                    timeOfDayStat.PlayCount = count;
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
