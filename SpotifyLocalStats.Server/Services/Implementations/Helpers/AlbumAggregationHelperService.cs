using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;

public class AlbumAggregationHelperService : IAlbumAggregationHelpersService
{
    private readonly ILogger<AlbumAggregationHelperService> _logger;
    private readonly SpotifyStatsContext _context;
    private readonly List<AggregatedAlbum> _aggregateAlbums;

    // really thinking this can be a helper class, where we pass in the aggergate we want to calc for, instead of having three separte aggreggate helpers that do pretty muhc smae thing? 
    public AlbumAggregationHelperService(ILogger<AlbumAggregationHelperService> logger, SpotifyStatsContext context, List<AggregatedAlbum> aggregatedAlbums)
    {
        _logger = logger;
        _context = context;
        _aggregateAlbums = _context.AggregatedAlbums.ToList(); // is this bad practice?, will this even compile?
    }

    public async Task RunCalculations()
    {
        await CalculateDrySpell();
        await CalculateLongestStreak();
        await CalculateMostTimesIn24Hours();
        await CalculateTopListeningDate();
    }

    public async Task CalculateTopListeningDate()
    {
        if (_aggregateAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        else
        {
            foreach (var aggAlbum in _aggregateAlbums)
            {
                var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id);

                var topDay = importedTracks.GroupBy(x => x.TimeStamp.Date).Select(g => new
                {
                    Date = g.Key,
                    PlayCount = g.Count()
                })
                     .OrderByDescending(g => g.PlayCount)
                     .FirstOrDefault();

                aggAlbum.TopListeningDate = topDay.Date;
            }
        }
    }

    /*
    public async Task CalculateUniqueAlbumsListened()
    {
        int uniqueAlbums = 0;

        if (_aggregateAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregateAlbums)
        {
            uniqueAlbums = _context.ImportedTracks
                .Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id)
                .Select(x => x.MasterMetadataTrackName)
                .Distinct()
                .Count();

            aggAlbum.User = uniqueTracks;
        }
    }
    */

    public async Task CalculateMostTimesIn24Hours()
    {
        // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)
        var playsIn24Hours = 0;

        //int timesListend24Hours = 0;

        if (_aggregateAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregateAlbums)
        {
            // need to get the max number of plays in any 24 hour period for this artist
            // set time frame from track time, then search 24 hours back, count numbver of times artist appears

            var allTracksOfAlbum = _context.ImportedTracks
                 .Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id).ToList();

            foreach (var trackOfAlbum in allTracksOfAlbum)
            {
                var trackTime = trackOfAlbum.TimeStamp;
                var startTime = trackTime.AddHours(-24);
                var nextPlaysIn24Hours = allTracksOfAlbum
                    .Where(x => x.TimeStamp >= startTime && x.TimeStamp < trackTime)
                    .Count();

                if (nextPlaysIn24Hours > playsIn24Hours)
                {
                    playsIn24Hours = nextPlaysIn24Hours;
                }
            }
            aggAlbum.MostTimesIn24Hours = playsIn24Hours;
        }
    }

    public async Task TimeOfDayStats()
    {
        // goal here is to get all tracks for artist then determine time of day stats by segmenting into morning, afternoon, evening, night?? or just hourly? BY the min? 
        // really need to determine how to split. 
        // also need to figure out whjat to return, maybe a dict for time of day and count?

        Dictionary<TimeSpan, int> timeOfDayCount;

        if (_aggregateAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        // loop through each artist, get artist tracks from imported tracks 
        foreach (var aggAlbum in _aggregateAlbums)
        {
            var artistAlbumTracks = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id)
                .ToList();

            // not sure if this will workl, as we need to span from 0000-1000 etc etc, if in this range, increment count

            timeOfDayCount = new Dictionary<TimeSpan, int> // thinking we use enum maybe iher instead of string?
            {
                { new TimeSpan(0, 0, 0) , 0}, // midnight
                { new TimeSpan(1,0,0), 0}, // 6am
                { new TimeSpan(2,0,0), 0}, // noon
                { new TimeSpan(3,0,0), 0}, // 6pm
                { new TimeSpan(4,0,0), 0}, // 11:59pm
                { new TimeSpan(5,0,0), 0}, // 11:59pm
                { new TimeSpan(6,0,0), 0}, // 11:59pm
                { new TimeSpan(7,0,0), 0}, // 11:59pm
                { new TimeSpan(8,0,0), 0}, // 11:59pm
                { new TimeSpan(9,0,0), 0}, // 11:59pm
                { new TimeSpan(10,0,0), 0}, // 11:59pm
                { new TimeSpan(11,0,0), 0} ,// 11:59pm
                { new TimeSpan(12,0,0), 0} ,// 11:59pm
                { new TimeSpan(13,0,0), 0}, // 11:59pm
                { new TimeSpan(14,0,0), 0}, // 11:59pm
                { new TimeSpan(15,0,0), 0}, // 11:59pm
                { new TimeSpan(16,0,0), 0}, // 11:59pm
                { new TimeSpan(17,0,0), 0}, // 11:59pm
                { new TimeSpan(18,0,0), 0} ,// 11:59pm
                { new TimeSpan(19,0,0), 0}, // 11:59pm
                { new TimeSpan(20,0,0), 0} ,// 11:59pm
                { new TimeSpan(21,0,0), 0} ,// 11:59pm
                { new TimeSpan(22,0,0), 0} ,// 11:59pm
                { new TimeSpan(23,0,0), 0} ,// 11:59pm
            };

            // then foreach track, determine time of day and increment count
            foreach (var track in artistAlbumTracks)
            {
                var trackTime = track.TimeStamp.TimeOfDay;

                // find closest hour slot
                var hourSlot = new TimeSpan(trackTime.Hours, 0, 0);
                if (timeOfDayCount.ContainsKey(hourSlot))
                {
                    timeOfDayCount[hourSlot]++;
                }
            }
            aggAlbum.TimeOfDayStats = timeOfDayCount;
        }
    }

    /*
    public async Task CalculateAlbumsListened()
    {
        // for each artist agg, get distinct album names from imported tracks for that artist and user
        var _aggregateAlbums = _context.AggregatedArtists.ToList();

        if (_aggregateAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregateAlbums)
        {
            var albumListenCount = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggAlbum.Artist.Name && x.UserId == aggAlbum.UserId)
                .GroupBy(x => x.MasterMetadataAlbumName)
                .ToList()
                .Count();

            aggAlbum.AlbumsListened = albumListenCount;
        }
    }*/

    // probably should get longest streak date start and end here. 
    public async Task CalculateLongestStreak()
    {
        // goal here is find the most amount of days in a row the artist was listened to

        var longestStreak = 0;
        var tempStreak = 0;

        var longestStreakEndDate = new DateTime();
        var longestStreakStartDate = new DateTime();


        if (_aggregateAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregateAlbums)
        {

            // ideally ordered by date from oldest to newest
            var albumTracks = _context.ImportedTracks.Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id).OrderBy(x => x.TimeStamp);

            var date = new DateTime();
            DateTime oneDateAhead = date.AddDays(1);

            foreach (var albumTrack in albumTracks)
            {
                // first iteration
                if (date.Date == DateTime.Parse("0001-01-01 00:00:00")) // ?? default date
                {
                    tempStreak = longestStreak++;

                    date = albumTrack.TimeStamp;
                    oneDateAhead = date.Date.AddDays(1);
                    continue;
                }
                else if (date == albumTrack.TimeStamp)
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

                    date = albumTrack.TimeStamp;
                    oneDateAhead = date.AddDays(1);
                }
                else
                {
                    longestStreakEndDate = date;
                    tempStreak = 0;
                }
            }
            // hopefully works
            aggAlbum.LongestStreakDays = longestStreak;
            aggAlbum.LongestStreakEndDate = longestStreakEndDate;
            aggAlbum.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
        }
    }

    public async Task CalculateDrySpell()
    {
        var dryStreak = 0;
        var dryStreakStartDate = new DateTime();
        var dryStreakEndDate = new DateTime();

        foreach (var aggAlbum in _aggregateAlbums)
        {
            var albumTracks = _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id).OrderBy(x => x.TimeStamp).ToList();

            // we have list of tracks in order, we just have to fin dlongest date between date values.. 
            for (var i = 0; i < albumTracks.Count(); i++)
            {
                if (dryStreak < (albumTracks[i].TimeStamp.Date - albumTracks[i - 1].TimeStamp.Date).Days)
                {
                    dryStreak = (albumTracks[i].TimeStamp.Date - albumTracks[i - 1].TimeStamp.Date).Days;
                    dryStreakStartDate = albumTracks[i].TimeStamp.Date;
                    dryStreakEndDate = albumTracks[i - 1].TimeStamp.Date;
                }
            }
            aggAlbum.LongestDrySpellEnd = dryStreakEndDate;
            aggAlbum.LongestStreakStartDate = dryStreakStartDate;
            aggAlbum.LongestDrySpell = dryStreak;
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
