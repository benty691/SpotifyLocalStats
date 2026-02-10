using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Models;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations.Helpers;

public sealed class AlbumAggregationHelperService : IAlbumAggregationHelpersService
{
    private readonly ILogger<AlbumAggregationHelperService> _logger;
    private readonly SpotifyStatsContext _context;
    private List<AggregatedAlbum> _aggregatedAlbums = new List<AggregatedAlbum>();

    // really thinking this can be a helper class, where we pass in the aggergate we want to calc for, instead of having three separte aggreggate helpers that do pretty muhc smae thing? 
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
        return await _context.AggregatedAlbums.ToListAsync();
    }

    public async Task RunCalculations()
    {
        CalculateLongestStreak();
        await InitializeAggregatedArtistsAsync();
        await CalculateDrySpell();
        await CalculateMostTimesIn24Hours();
        await CalculateTopListeningDate();
        await TimeOfDayStats();
    }

    private async Task CalculateTopListeningDate()
    {
        if (_aggregatedAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        else
        {
            foreach (var aggAlbum in _aggregatedAlbums)
            {
                var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id);

                var topDay = await importedTracks.GroupBy(x => x.TimeStamp.Date).Select(g => new
                {
                    Date = g.Key,
                    PlayCount = g.Count()
                })
                     .OrderByDescending(g => g.PlayCount)
                     .FirstOrDefaultAsync();

                if (topDay == null)
                    throw new Exception($"{topDay} is null");

                aggAlbum.TopListeningDate = topDay.Date;
            }
        }
    }

    /*
    public async Task CalculateUniqueAlbumsListened()
    {
        int uniqueAlbums = 0;

        if (_aggregatedAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregatedAlbums)
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

    private async Task CalculateMostTimesIn24Hours()
    {
        // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)
        var playsIn24Hours = 0;

        //int timesListend24Hours = 0;

        if (_aggregatedAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregatedAlbums)
        {
            // need to get the max number of plays in any 24 hour period for this artist
            // set time frame from track time, then search 24 hours back, count numbver of times artist appears

            var allTracksOfAlbum = await _context.ImportedTracks
                 .Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id).ToListAsync();

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

    private async Task TimeOfDayStats()
    {
        // goal here is to get all tracks for artist then determine time of day stats by segmenting into morning, afternoon, evening, night?? or just hourly? BY the min? 
        // really need to determine how to split. 
        // also need to figure out whjat to return, maybe a dict for time of day and count?

        TimeOfDayStat<AggregatedAlbum> timeOfDayCount;

        if (_aggregatedAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        // loop through each artist, get artist tracks from imported tracks 
        foreach (var aggAlbum in _aggregatedAlbums)
        {
            var artistAlbumTracks = await _context.ImportedTracks
                .Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id)
                .ToListAsync();

            // not sure if this will workl, as we need to span from 0000-1000 etc etc, if in this range, increment count
            // need to get old stats, then update them with new, or make old obselete, or somehting?? 
            // create new for now, but we need to delete all old after we get new... 

            // then foreach track, determine time of day and increment count
            foreach (var track in artistAlbumTracks)
            {
                // dont create a new one eveyrtime, just increase count by 1 if it exists, if nt create it
                var timeOfDayStatsForUser = await _context.AlbumTimeOfDaysStats.Where(x => x.Aggregate.User.Id == track.UserId).ToListAsync();

                if (timeOfDayStatsForUser.Count != 0)
                {
                    var todSameAsTrack = timeOfDayStatsForUser.Where(x => x.TimeOfDay == track.TimeStamp.Hour).First();

                    if (todSameAsTrack is null) //???
                    {
                        timeOfDayCount = new TimeOfDayStat<AggregatedAlbum>()
                        {
                            CreatedAt = DateTime.UtcNow,
                            Aggregate = aggAlbum,
                            AggregateId = aggAlbum.Id,
                            TimeOfDay = track.TimeStamp.Hour, // this is a in value 0 - 23
                            PlayCount = 1
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

    /*
    public async Task CalculateAlbumsListened()
    {
        // for each artist agg, get distinct album names from imported tracks for that artist and user
        var _aggregatedAlbums = _context.AggregatedArtists.ToList();

        if (_aggregatedAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregatedAlbums)
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
    private void CalculateLongestStreak()
    {
        // goal here is find the most amount of days in a row the artist was listened to

        var longestStreak = 0;
        var tempStreak = 0;

        var longestStreakEndDate = new DateTime();

        if (_aggregatedAlbums.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggAlbum in _aggregatedAlbums)
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

    private async Task CalculateDrySpell()
    {
        var dryStreak = 0;
        var dryStreakStartDate = new DateTime();
        var dryStreakEndDate = new DateTime();

        foreach (var aggAlbum in _aggregatedAlbums)
        {
            var albumTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggAlbum.Album.Name && x.User.Id == aggAlbum.User.Id).OrderBy(x => x.TimeStamp).ToListAsync();

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
