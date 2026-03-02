using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Models;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;

public sealed class TrackAggregationHelpersService : ITrackAggregationHelpersService
{
    private readonly ILogger<TrackAggregationHelpersService> _logger;
    private readonly SpotifyStatsContext _context;
    private List<AggregatedTrack> _aggregatedTracks = new List<AggregatedTrack>();

    private string DEFAULT_DATE = "0001-01-01 00:00:00";

    // really thinking this can be a helper class, where we pass in the aggergate we want to calc for, instead of having three separte aggreggate helpers that do pretty muhc smae thing? 
    public TrackAggregationHelpersService(ILogger<TrackAggregationHelpersService> logger, SpotifyStatsContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task InitializeAggregatedTracksAsync()
    {
        _aggregatedTracks = await GetAggregatedTracks();
    }

    private async Task<List<AggregatedTrack>> GetAggregatedTracks()
    {
        var aggTracks = await _context.AggregatedTracks.ToListAsync();

        if (!aggTracks.Any())
        {
            throw new InvalidOperationException("No aggregated tracks found.");
        }
        return aggTracks;
    }

    public async Task RunCalculations()
    {
        await InitializeAggregatedTracksAsync();
        await CalculateLongestStreak();
        await CalculateDrySpell();
        await CalculateMostTimesIn24Hours();
        await CalculateTopListeningDate();
        await TimeOfDayStats();
    }

    private async Task CalculateTopListeningDate()
    {
        //var _aggregatedTracks = _context.AggregatedArtists.ToList();

        if (_aggregatedTracks.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        else
        {
            foreach (var aggTrack in _aggregatedTracks)
            {
                var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.UserId == aggTrack.UserId);

                var topDay = await importedTracks.GroupBy(x => x.TimeStamp.Date).Select(g => new
                {
                    Date = g.Key,
                    PlayCount = g.Count()
                })
                     .OrderByDescending(g => g.PlayCount)
                     .FirstOrDefaultAsync();

                if (topDay == null)
                    throw new Exception($"{topDay} is null");

                aggTrack.TopListeningDate = topDay.Date;
            }
        }
    }

    private async Task CalculateMostTimesIn24Hours()
    {
        var playsIn24Hours = 0;

        if (_aggregatedTracks.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggTrack in _aggregatedTracks)
        {
            var tracksOfTracks = await _context.ImportedTracks
                 .Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.UserId == aggTrack.UserId).OrderBy(x => x.TimeStamp).ToListAsync();

            foreach (var track in tracksOfTracks)
            {
                var trackTime = track.TimeStamp;
                var startTime = trackTime.AddHours(-24);
                var nextPlaysIn24Hours = tracksOfTracks
                    .Where(x => x.TimeStamp >= startTime && x.TimeStamp < trackTime)
                    .Count();

                if (nextPlaysIn24Hours > playsIn24Hours)
                {
                    playsIn24Hours = nextPlaysIn24Hours;
                }
            }
            aggTrack.MostTimesIn24Hours = playsIn24Hours;
        }
    }

    private async Task TimeOfDayStats()
    {
        // goal here is to get all tracks for artist then determine time of day stats by segmenting into morning, afternoon, evening, night?? or just hourly? BY the min? 
        // really need to determine how to split. 
        // also need to figure out whjat to return, maybe a dict for time of day and count?

        TimeOfDayStat<AggregatedTrack> timeOfDayCount;

        if (_aggregatedTracks.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        // loop through each artist, get artist tracks from imported tracks 
        foreach (var aggTrack in _aggregatedTracks)
        {
            var artistTracks = await _context.ImportedTracks
                .Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.UserId == aggTrack.UserId)
                .ToListAsync();

            var timeOfDayStatsForUser = await _context.TrackTimeOfDaysStats.Where(x => x.Aggregate.UserId == aggTrack.UserId && x.Aggregate.Id == aggTrack.Id).ToListAsync();

            // not sure if this will workl, as we need to span from 0000-1000 etc etc, if in this range, increment count
            // need to get old stats, then update them with new, or make old obselete, or somehting?? 
            // create new for now, but we need to delete all old after we get new... 

            // then foreach track, determine time of day and increment count
            foreach (var track in artistTracks)
            {
                // dont create a new one eveyrtime, just increase count by 1 if it exists, if nt create it


                if (timeOfDayStatsForUser.Count != 0)
                {
                    var todSameAsTrack = timeOfDayStatsForUser.Where(x => x.TimeOfDay == track.TimeStamp.Hour).First();

                    if (todSameAsTrack is null) //???
                    {
                        timeOfDayCount = new TimeOfDayStat<AggregatedTrack>(aggTrack.Id, track.TimeStamp.Hour, 1)
                        {
                            Aggregate = aggTrack,
                        };
                    }
                    else
                    {
                        todSameAsTrack.PlayCount = +1;
                        todSameAsTrack.LastUpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }

    // probably should get longest streak date start and end here.
    private async Task CalculateLongestStreak()
    {
        // goal here is find the most amount of days in a row the artist was listened to
        foreach (var aggTrack in _aggregatedTracks) //O(n)
        {
            var longestStreak = 1;
            var tempStreak = 0;
            var longestStreakEndDate = new DateTime();

            var artistTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.UserId == aggTrack.UserId).OrderBy(x => x.TimeStamp).ToListAsync(); // O(n)

            var date = new DateTime();
            DateTime oneDateAhead = date.AddDays(1);

            foreach (var artistTrack in artistTracks) //O(n)
            {
                //first iteration, using defauilt date as check
                if (date.Date == DateTime.Parse(DEFAULT_DATE)) // ?? default date
                {
                    tempStreak = longestStreak;

                    date = artistTrack.TimeStamp;
                    oneDateAhead = date.AddDays(1);

                    longestStreakEndDate = date;
                    continue;
                }
                else if (date.Date == artistTrack.TimeStamp.Date)
                {
                    longestStreakEndDate = artistTrack.TimeStamp.Date;
                    continue;
                }
                else if (oneDateAhead.Date == artistTrack.TimeStamp.Date)
                {
                    tempStreak++;
                    if (tempStreak > longestStreak)
                    {
                        longestStreak = tempStreak;
                        longestStreakEndDate = date;
                    }

                    date = artistTrack.TimeStamp;
                    oneDateAhead = date.AddDays(1);
                }
                else
                {
                    if (tempStreak > longestStreak)
                    {
                        longestStreak = tempStreak;
                        longestStreakEndDate = date;
                    }
                    tempStreak = 0;

                    date = new DateTime();
                }
            }
            aggTrack.LongestStreakDays = longestStreak;
            aggTrack.LongestStreakEndDate = longestStreakEndDate;
            aggTrack.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
        }
    }

    private async Task CalculateDrySpell()
    {
        foreach (var aggTrack in _aggregatedTracks)
        {
            var drySpellStartDate = new DateTime();
            var dryStreakEndDate = new DateTime();
            var drySpell = 0;

            var artistTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.UserId == aggTrack.UserId).OrderBy(x => x.TimeStamp).ToListAsync();

            for (var i = 0; i < artistTracks.Count; i++)
            {
                if (i == 0)
                {
                    drySpellStartDate = artistTracks[i].TimeStamp.Date;
                    dryStreakEndDate = artistTracks[i].TimeStamp.Date;
                    continue;
                }

                if (drySpell < (artistTracks[i].TimeStamp.Date - artistTracks[i - 1].TimeStamp.Date).Days)
                {
                    drySpell = (artistTracks[i].TimeStamp.Date - artistTracks[i - 1].TimeStamp.Date).Days;
                    drySpellStartDate = artistTracks[i - 1].TimeStamp.Date;
                    dryStreakEndDate = artistTracks[i].TimeStamp.Date;
                }
            }
            aggTrack.LongestDrySpellEnd = dryStreakEndDate;
            aggTrack.LongestDrySpellStart = drySpellStartDate;
            aggTrack.LongestDrySpell = drySpell;
        }
    }
}
