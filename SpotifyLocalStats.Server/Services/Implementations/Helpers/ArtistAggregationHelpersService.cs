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
        return await _context.AggregatedArtists.ToListAsync();
    }

    public async Task RunCalculations()
    {
        await InitializeAggregatedTracksAsync();
        CalculateLongestStreak();
        CalculateUniqueArtistTracksListened();
        CalculateAlbumsListened();
        await CalculateDrySpell();
        await CalculateMostTimesIn24Hours();
        await CalculateTopListeningDate();
        await TimeOfDayStats();
    }

    private async Task CalculateTopListeningDate()
    {
        //var _aggregateArtists = _context.AggregatedArtists.ToList();

        if (_aggregateArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        else
        {
            foreach (var aggArtist in _aggregateArtists)
            {
                var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id);

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
    }

    private void CalculateUniqueArtistTracksListened()
    {
        int uniqueTracks = 0; 

        if (_aggregateArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in _aggregateArtists)
        {
            uniqueTracks = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                .Select(x => x.MasterMetadataTrackName)
                .Distinct()
                .Count();

            aggArtist.UniqueTracksPlayed = uniqueTracks;
        }
    }

    private async Task CalculateMostTimesIn24Hours()
    {
        // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)
        var playsIn24Hours = 0;

        //int timesListend24Hours = 0;

        if (_aggregateArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in _aggregateArtists)
        {
            // need to get the max number of plays in any 24 hour period for this artist
            // set time frame from track time, then search 24 hours back, count numbver of times artist appears

           var allTracksOfArtist = await _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id).ToListAsync();
            
            foreach(var trackOfArtist in allTracksOfArtist)
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
        // goal here is to get all tracks for artist then determine time of day stats by segmenting into morning, afternoon, evening, night?? or just hourly? BY the min? 
        // really need to determine how to split. 
        // also need to figure out whjat to return, maybe a dict for time of day and count?

        TimeOfDayStat<AggregatedArtist> timeOfDayCount; 

        if (_aggregateArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        // loop through each artist, get artist tracks from imported tracks 
        foreach (var aggArtist in _aggregateArtists)
        {
            var artistTracks = await _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                .ToListAsync();

            // not sure if this will workl, as we need to span from 0000-1000 etc etc, if in this range, increment count
            // need to get old stats, then update them with new, or make old obselete, or somehting?? 
            // create new for now, but we need to delete all old after we get new... 

            // then foreach track, determine time of day and increment count
            foreach (var track in artistTracks)
            {
                // dont create a new one eveyrtime, just increase count by 1 if it exists, if nt create it
                var timeOfDayStatsForUser = await _context.ArtistTimeOfDaysStats.Where(x => x.Aggregate!.User.Id == track.UserId).ToListAsync();

                if (timeOfDayStatsForUser.Count != 0)
                {
                    var todSameAsTrack = timeOfDayStatsForUser.Where(x => x.TimeOfDay == track.TimeStamp.Hour).First();

                    if (todSameAsTrack is null) //???
                    {
                        timeOfDayCount = new TimeOfDayStat<AggregatedArtist>(aggArtist.Id, track.TimeStamp.Hour, 1)
                        {
                            Aggregate = aggArtist,
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

    private void CalculateAlbumsListened()
    {
        // for each artist agg, get distinct album names from imported tracks for that artist and user
        if (_aggregateArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in _aggregateArtists)
        {
            var albumListenCount = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.UserId == aggArtist.User.Id)
                .GroupBy(x => x.MasterMetadataAlbumName)
                .ToList()
                .Count();

            aggArtist.AlbumsListened = albumListenCount;
        }
    }

    // probably should get longest streak date start and end here. 
    private void CalculateLongestStreak()
    {
        // goal here is find the most amount of days in a row the artist was listened to
        var longestStreak = 0;
        var tempStreak = 0;

        var longestStreakEndDate = new DateTime();

        if (_aggregateArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in _aggregateArtists)
        {

            // ideally ordered by date from oldest to newest
            var artistTracks = _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id).OrderBy(x => x.TimeStamp);
            
            var date = new DateTime();
            DateTime oneDateAhead = date.AddDays(1);

            foreach (var artistTrack in artistTracks)
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
            aggArtist.LongestStreakDays = longestStreak;
            aggArtist.LongestStreakEndDate = longestStreakEndDate;
            aggArtist.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
        }
    }

    private async Task CalculateDrySpell()
    {
        var drySpell = 0;
        var drySpellStartDate = new DateTime();
        var dryStreakEndDate = new DateTime();

        foreach (var aggArtist in _aggregateArtists)
        {
            var artistTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id).OrderBy(x => x.TimeStamp).ToListAsync();

            // we have list of tracks in order, we just have to find longest date between date values.. 
            for (var i = 1; i < artistTracks.Count(); i++)
            {
                if (drySpell < (artistTracks[i].TimeStamp.Date - artistTracks[i - 1].TimeStamp.Date).Days)
                {
                    drySpell = (artistTracks[i].TimeStamp.Date - artistTracks[i - 1].TimeStamp.Date).Days;
                    drySpellStartDate = artistTracks[i].TimeStamp.Date;
                    dryStreakEndDate = artistTracks[i - 1].TimeStamp.Date;
                }
            }
            aggArtist.LongestDrySpellEnd = dryStreakEndDate;
            aggArtist.LongestDrySpellStart = drySpellStartDate;
            aggArtist.LongestDrySpell = drySpell;
        }
    }
}
