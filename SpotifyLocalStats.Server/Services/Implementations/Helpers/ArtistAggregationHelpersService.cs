using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Models;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;

public sealed class ArtistAggregationHelpersService : IArtistAggregationHelpersService
{
    private readonly ILogger<ArtistAggregationHelpersService> _logger;
    private readonly SpotifyStatsContext _context;
    private List<AggregatedArtist> _aggregateArtists = new List<AggregatedArtist>();

    private string DEFAULT_DATE = "0001-01-01 00:00:00";


    // really thinking this can be a helper class, where we pass in the aggergate we want to calc for, instead of having three separte aggreggate helpers that do pretty muhc smae thing? 
    public ArtistAggregationHelpersService(ILogger<ArtistAggregationHelpersService> logger, SpotifyStatsContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task InitializeAggregatedTracksAsync()
    {
        _aggregateArtists = await getAggregatedArtists();
    }

    private async Task<List<AggregatedArtist>> getAggregatedArtists()
    {
        var aggList = await _context.AggregatedArtists.ToListAsync();

        if (aggList.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }
        return aggList;
    }

    public async Task RunCalculations()
    {
        await InitializeAggregatedTracksAsync();
        await CalculateLongestStreak();
        await CalculateUniqueArtistTracksListened();
        CalculateAlbumsListened();
        await CalculateDrySpell();
        await CalculateMostTimesIn24Hours();
        await CalculateTopListeningDate();
        await TimeOfDayStats();
    }

    private async Task CalculateTopListeningDate()
    {

        foreach (var aggArtist in _aggregateArtists)
        {
            var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId);

            var topDay = await importedTracks.GroupBy(x => x.TimeStamp.Date).Select(g => new
            {
                Date = g.Key,
                PlayCount = g.Count()
            })
                    .OrderByDescending(g => g.PlayCount)
                    .FirstOrDefaultAsync();

            if (topDay == null)
                throw new Exception($"{topDay} is null");

            aggArtist.TopListeningDate = topDay.Date;

        }
    }

    private async Task CalculateUniqueArtistTracksListened()
    {
        int uniqueTracks = 0;

        foreach (var aggArtist in _aggregateArtists)
        {
            uniqueTracks = await _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId)
                .Select(x => x.MasterMetadataTrackName)
                .Distinct()
                .CountAsync();

            aggArtist.UniqueTracksPlayed = uniqueTracks;
        }
    }

    private async Task CalculateMostTimesIn24Hours()
    {
        // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)
        var playsIn24Hours = 1;

        foreach (var aggArtist in _aggregateArtists)
        {
            // need to get the max number of plays in any 24 hour period for this artist
            // set time frame from track time, then search 24 hours back, count numbver of times artist appears

            var allTracksOfArtist = await _context.ImportedTracks
                 .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId).OrderBy(x => x.TimeStamp).ToListAsync();

            foreach (var trackOfArtist in allTracksOfArtist)
            {
                var trackTime = trackOfArtist.TimeStamp;
                var startTime = trackTime.AddHours(-24);
                var nextPlaysIn24Hours = allTracksOfArtist
                    .Where(x => x.TimeStamp >= startTime && x.TimeStamp < trackTime)
                    .Count();

                if (nextPlaysIn24Hours > playsIn24Hours)
                {
                    playsIn24Hours = nextPlaysIn24Hours;
                }
            }
            aggArtist.MostTimesIn24Hours = playsIn24Hours;
        }
    }

    private async Task TimeOfDayStats()
    {
        // loop through each artist, get artist tracks from imported tracks 
        foreach (var aggArtist in _aggregateArtists)
        {
            var artistTracks = await _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId)
                .ToListAsync();

            var timeOfDayStatsForUser = _context.ArtistTimeOfDaysStats.Where(x => x.Aggregate.Id == aggArtist.Id && aggArtist.UserId == x.Aggregate.UserId).ToDictionary(x => x.TimeOfDay, x => x);

            foreach (var track in artistTracks)
            {
                var todSameAsTrack = timeOfDayStatsForUser.TryGetValue(track.TimeStamp.Hour, out var timeOfDayStat);

                if (!todSameAsTrack) //i.e this time of day for this artiost doesnt exist yet
                {
                    var timeOfDay = new TimeOfDayStat<AggregatedArtist>(aggArtist.Id, track.TimeStamp.Hour, 1)
                    {
                        Aggregate = aggArtist,
                    };
                    await _context.ArtistTimeOfDaysStats.AddAsync(timeOfDay);
                    timeOfDayStatsForUser[track.TimeStamp.Hour] = timeOfDay;
                }
                else
                {
                    timeOfDayStat.PlayCount += 1;
                    timeOfDayStat.LastUpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    private void CalculateAlbumsListened()
    {
        // for each artist agg, get distinct album names from imported tracks for that artist and user
        foreach (var aggArtist in _aggregateArtists)
        {
            var albumListenCount = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId)
                .GroupBy(x => x.MasterMetadataAlbumName)
                .ToList()
                .Count();

            aggArtist.AlbumsListened = albumListenCount;
        }
    }

    // probably should get longest streak date start and end here. 
    private async Task CalculateLongestStreak()
    {
        // goal here is find the most amount of days in a row the artist was listened to
        foreach (var aggArtist in _aggregateArtists) //O(n)
        {
            var longestStreak = 1;
            var tempStreak = 0;
            var longestStreakEndDate = new DateTime();

            // ideally ordered by date from oldest to newest
            var artistTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId).OrderBy(x => x.TimeStamp).ToListAsync(); // O(n)

            var date = new DateTime();
            DateTime oneDateAhead = date.AddDays(1);

            foreach (var artistTrack in artistTracks) //O(n)
            {
                //first iteration, using defauilt date as check
                if (date.Date == DateTime.Parse(DEFAULT_DATE)) // ?? default date
                {
                    // set temp streak to longeststreak +1 (= 1) 
                    tempStreak = longestStreak;

                    // setting date to the time we first listened to this track 
                    date = artistTrack.TimeStamp;
                    oneDateAhead = date.AddDays(1);


                    longestStreakEndDate = date;
                    continue;
                }
                else if (date.Date == artistTrack.TimeStamp.Date)
                {
                    // same day, we just move onto next track
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
            // hopefully works
            aggArtist.LongestStreakDays = longestStreak;
            aggArtist.LongestStreakEndDate = longestStreakEndDate;
            aggArtist.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
        }
    }

    private async Task CalculateDrySpell()
    {
        // the idea behind this is as follows;
        // - We start calculating from the first time the user listend to an artist. 
        // - So, Ifd we first listened on 19/03/2023, this is the first day we can register dry spells from
        // - Dry spell is calculated as time from last listen to next listen, ie biggets ghap between listens. 
        // - Ideally in future we have this as an ongoing value that if they last listened in 2024, and we are in, dry spell is ~2 years (pending this dry spell is longer than any previous one) 

        foreach (var aggArtist in _aggregateArtists)
        {
            var drySpellStartDate = new DateTime();
            var dryStreakEndDate = new DateTime();
            var drySpell = 0;

            var artistTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.UserId).OrderBy(x => x.TimeStamp).ToListAsync();
            // need to investigate if this is ordered correctly 

            // we have list of tracks in order, we just have to find longest date between date values.. 
            for (var i = 0; i < artistTracks.Count; i++)
            {
                if (i == 0)
                {
                    // first time this artist was listened to by the user. 
                    drySpellStartDate = artistTracks[i].TimeStamp.Date;
                    // setting end date such that if this is the only time the artist was listend to.. but we probbaly will set up cron jobs that update table records to adjust their sry streak end date.. 
                    dryStreakEndDate = artistTracks[i].TimeStamp.Date;
                    continue;
                }

                if (drySpell < (artistTracks[i].TimeStamp.Date - artistTracks[i - 1].TimeStamp.Date).Days)
                {
                    drySpell = (artistTracks[i].TimeStamp.Date - artistTracks[i - 1].TimeStamp.Date).Days;
                    drySpellStartDate = artistTracks[i - 1].TimeStamp.Date;

                    // intend to update such that this is tied to current day aswell, but for now only using historical data. 
                    dryStreakEndDate = artistTracks[i].TimeStamp.Date;
                }
            }
            aggArtist.LongestDrySpellEnd = dryStreakEndDate;
            aggArtist.LongestDrySpellStart = drySpellStartDate;
            aggArtist.LongestDrySpell = drySpell;
        }
    }
}
