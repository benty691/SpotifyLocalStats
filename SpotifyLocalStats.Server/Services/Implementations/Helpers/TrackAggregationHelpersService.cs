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
        return await _context.AggregatedTracks.ToListAsync();
    }

    public async Task RunCalculations()
    {
        await InitializeAggregatedTracksAsync();
        CalculateLongestStreak();
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
        // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)
        var playsIn24Hours = 0;

        //int timesListend24Hours = 0;

        if (_aggregatedTracks.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggTrack in _aggregatedTracks)
        {
            // need to get the max number of plays in any 24 hour period for this artist
            // set time frame from track time, then search 24 hours back, count numbver of times artist appears

            var tracksOfTracks = await _context.ImportedTracks
                 .Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.UserId == aggTrack.UserId).ToListAsync();

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

            // not sure if this will workl, as we need to span from 0000-1000 etc etc, if in this range, increment count
            // need to get old stats, then update them with new, or make old obselete, or somehting?? 
            // create new for now, but we need to delete all old after we get new... 

            // then foreach track, determine time of day and increment count
            foreach (var track in artistTracks)
            {
                // dont create a new one eveyrtime, just increase count by 1 if it exists, if nt create it
                var timeOfDayStatsForUser = await _context.TrackTimeOfDaysStats.Where(x => x.Aggregate.UserId == track.UserId).ToListAsync();

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
    private void CalculateLongestStreak()
    {
        // goal here is find the most amount of days in a row the artist was listened to
        var longestStreak = 0;
        var tempStreak = 0;

        var longestStreakEndDate = new DateTime();

        if (_aggregatedTracks.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggTrack in _aggregatedTracks)
        {

            // ideally ordered by date from oldest to newest
            var tracks = _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id).OrderBy(x => x.TimeStamp);

            var date = new DateTime();
            DateTime oneDateAhead = date.AddDays(1);

            foreach (var artistTrack in tracks)
            {
                if (date.Date == DateTime.Parse("0001-01-01 00:00:00")) // ?? default date
                {
                    tempStreak = longestStreak++;

                    date = artistTrack.TimeStamp;
                    oneDateAhead = date.Date.AddDays(1);
                    continue;
                }
                else if (date == artistTrack.TimeStamp)
                {
                    // same day, we just move onto next track
                    continue;
                }
                else if (date == oneDateAhead)
                {
                    tempStreak++;
                    if (tempStreak > longestStreak)
                    {
                        longestStreak = tempStreak;
                    }

                    date = artistTrack.TimeStamp;
                    oneDateAhead = date.AddDays(1);
                }
                else
                {
                    longestStreakEndDate = date;
                    tempStreak = 0;
                }
            }
            // hopefully works
            aggTrack.LongestStreakDays = longestStreak;
            aggTrack.LongestStreakEndDate = longestStreakEndDate;
            aggTrack.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
        }
    }

    private async Task CalculateDrySpell()
    {
        var dryStreak = 0;
        var dryStreakStartDate = new DateTime();
        var dryStreakEndDate = new DateTime();

        foreach (var aggTrack in _aggregatedTracks)
        {
            var tracks = await _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id).OrderBy(x => x.TimeStamp).ToListAsync();

            // we have list of tracks in order, we just have to fin dlongest date between date values.. 
            for (var i = 1; i < tracks.Count(); i++)
            {
                if (dryStreak < (tracks[i].TimeStamp.Date - tracks[i - 1].TimeStamp.Date).Days)
                {
                    dryStreak = (tracks[i].TimeStamp.Date - tracks[i - 1].TimeStamp.Date).Days;
                    dryStreakStartDate = tracks[i].TimeStamp.Date;
                    dryStreakEndDate = tracks[i - 1].TimeStamp.Date;
                }
            }
            aggTrack.LongestDrySpellEnd = dryStreakEndDate;
            aggTrack.LongestDrySpellStart = dryStreakStartDate;
            aggTrack.LongestDrySpell = dryStreak;
        }
    }
}
