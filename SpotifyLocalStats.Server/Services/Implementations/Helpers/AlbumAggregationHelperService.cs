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

    private string DEFAULT_DATE = "0001-01-01 00:00:00";

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

    public async Task RunCalculations()
    {
        await InitializeAggregatedArtistsAsync();
        await CalculateLongestStreak();
        await CalculateDrySpell();
        await CalculateMostTimesIn24Hours();
        await CalculateTopListeningDate();
        await TimeOfDayStats();
    }

    private async Task CalculateTopListeningDate()
    {
        foreach (var aggAlbum in _aggregatedAlbums)
        {
            var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.User.Id == aggAlbum.UserId);

            var topDay = await importedTracks.GroupBy(x => x.TimeStamp.Date).Select(g => new
            {
                Date = g.Key,
                PlayCount = g.Count()
            })
                    .OrderByDescending(g => g.PlayCount)
                    .FirstOrDefaultAsync();

            if (topDay == null)
                throw new ArgumentNullException($"{topDay} is null");

            aggAlbum.TopListeningDate = topDay.Date;
        }
    }

    private async Task CalculateMostTimesIn24Hours()
    {
        var playsIn24Hours = 1;

        foreach (var aggAlbum in _aggregatedAlbums)
        {

            var allTracksOfAlbum = await _context.ImportedTracks
                 .Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.UserId == aggAlbum.UserId).OrderBy(x => x.TimeStamp).ToListAsync();

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
        foreach (var aggAlbum in _aggregatedAlbums)
        {
            var albumTracks = await _context.ImportedTracks
                .Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.UserId == aggAlbum.UserId)
                .ToListAsync();

            var timeOfDayStatsForUser = _context.AlbumTimeOfDaysStats.Where(x => x.Aggregate.UserId == aggAlbum.UserId).ToDictionary(x => x.TimeOfDay);

            foreach (var albumTrack in albumTracks)
            {
                var todSameAsTrack = timeOfDayStatsForUser.TryGetValue(albumTrack.TimeStamp.Hour, out var timeOfDayStat);

                if (!todSameAsTrack) //i.e this time of day for this album doenst exist yet
                {
                    var timeOfDay = new TimeOfDayStat<AggregatedAlbum>(aggAlbum.Id, albumTrack.TimeStamp.Hour, 1)
                    {
                        Aggregate = aggAlbum,
                    };
                    await _context.AlbumTimeOfDaysStats.AddAsync(timeOfDay);
                    timeOfDayStatsForUser[albumTrack.TimeStamp.Hour] = timeOfDay;
                }
                else
                {
                    timeOfDayStat.PlayCount += 1;
                    timeOfDayStat.LastUpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    private async Task CalculateLongestStreak()
    {
        var longestStreak = 0;
        var tempStreak = 0;

        var longestStreakEndDate = new DateTime();

        foreach (var aggAlbum in _aggregatedAlbums) //O(n)
        {
            var albumTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.UserId == aggAlbum.UserId).OrderBy(x => x.TimeStamp).ToListAsync(); // O(n) 

            var date = new DateTime();
            DateTime oneDateAhead = date.AddDays(1);

            foreach (var albumTrack in albumTracks) //O(n)
            {
                //first iteration, using defauilt date as check
                if (date.Date == DateTime.Parse(DEFAULT_DATE)) // ?? default date
                {
                    tempStreak = longestStreak++;

                    // setting date to the time we first listened to this track 
                    date = albumTrack.TimeStamp;
                    oneDateAhead = date.AddDays(1);

                    if (albumTracks.Count == 1)
                    {
                        longestStreakEndDate = date;
                    }
                    continue;
                }
                else if (date.Date == albumTrack.TimeStamp.Date)
                {
                    // same day, we just move onto next track
                    longestStreakEndDate = albumTrack.TimeStamp.Date;
                    continue;
                }
                else if (oneDateAhead.Date == albumTrack.TimeStamp.Date)
                {
                    tempStreak++;
                    if (tempStreak > longestStreak)
                    {
                        longestStreak = tempStreak;
                        longestStreakEndDate = date;
                    }

                    date = albumTrack.TimeStamp;
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
            aggAlbum.LongestStreakDays = longestStreak;
            aggAlbum.LongestStreakEndDate = longestStreakEndDate;
            aggAlbum.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
        }
    }

    private async Task CalculateDrySpell()
    {
        foreach (var aggAlbum in _aggregatedAlbums)
        {
            var drySpellStartDate = new DateTime();
            var dryStreakEndDate = new DateTime();
            var drySpell = 0;

            var albumTracks = await _context.ImportedTracks.Where(x => x.MasterMetadataAlbumName == aggAlbum.Album.Name && x.UserId == aggAlbum.UserId).OrderBy(x => x.TimeStamp).ToListAsync();

            for (var i = 0; i < albumTracks.Count; i++)
            {
                if (i == 0)
                {
                    drySpellStartDate = albumTracks[i].TimeStamp.Date;
                    dryStreakEndDate = albumTracks[i].TimeStamp.Date;
                    continue;
                }

                if (drySpell < (albumTracks[i].TimeStamp.Date - albumTracks[i - 1].TimeStamp.Date).Days)
                {
                    drySpell = (albumTracks[i].TimeStamp.Date - albumTracks[i - 1].TimeStamp.Date).Days;
                    drySpellStartDate = albumTracks[i - 1].TimeStamp.Date;
                    dryStreakEndDate = albumTracks[i].TimeStamp.Date;
                }
            }
            aggAlbum.LongestDrySpellEnd = dryStreakEndDate;
            aggAlbum.LongestDrySpellStart = drySpellStartDate;
            aggAlbum.LongestDrySpell = drySpell;
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
