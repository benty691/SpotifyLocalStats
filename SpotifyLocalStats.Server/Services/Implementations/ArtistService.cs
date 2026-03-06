using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;
using WebApi.Data.DTOs.NewFolder;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly SpotifyStatsContext _context;
    private readonly ILogger<ArtistService> _logger;

    public ArtistService(SpotifyStatsContext spotifyStatsContext, ILogger<ArtistService> logger)
    {
        _context = spotifyStatsContext;
        _logger = logger;
    }

    public async Task<AggregateArtistDto> GetArtistDetailsAsync(Guid userId, Guid aggregateId)
    {
        var aggArtist = await _context.AggregatedArtists.Where(x => x.Artist.Id == aggregateId && x.UserId == userId).SingleOrDefaultAsync();

        return new AggregateArtistDto()
        {
            FirstListened = aggArtist.DateTimeFirstListened,
            LastListened = aggArtist.DateTimeLastListened,
            LongestDrySpell = aggArtist.LongestDrySpell,
            LongestDrySpellEnd = aggArtist.LongestDrySpellEnd,
            LongestDrySpellStart = aggArtist.LongestDrySpellStart,
            LongestStreak = aggArtist.LongestStreakDays,
            LongestStreakStart = aggArtist.LongestStreakStartDate,
            LongestStreakEnd = aggArtist.LongestStreakEndDate,
            MinsListened = aggArtist.MinsListened,
            MostTimesIn24Hours = aggArtist.MostTimesIn24Hours,
            Name = aggArtist.Artist.Name,
            PlayCount = aggArtist.PlayCount,
            TimeOfDayStats = aggArtist.TimeOfDayStats
                        .Select(t => new TimeOfDayStatDto<AggregatedArtist>
                        {
                            AggregateId = aggArtist.Id,
                            TimeOfDay = t.TimeOfDay,
                            PlayCount = t.PlayCount,
                            LastUpdatedAt = t.LastUpdatedAt
                        }).ToList(),
            TopListeningDate = aggArtist.TopListeningDate,
            TotalRecords = 1
        };
    }

    public async Task<ArtistTimeframeResponseDto> GetArtistListenTimeframe(ArtistTimeframeRangeDto input)
    {
        // goal; of this is to get all the imported tracks with the artist if of this agg artist with the user id, break it down into a timeline showing listening from first listend to today 
        // user can filter how they like, this is passed in via the controller, and we filter the respons ebased upon this range, ie, from 2020 - 2023 show listening poatterns, 

        var aggArtist = await _context.AggregatedArtists
            .Where(x => x.Artist.Id == input.aggArtistId && x.UserId == input.userId)
            .SingleOrDefaultAsync();

        var importedTracksForArtist = await _context.ImportedTracks
            .Where(x => x.UserId == input.userId && x.MasterMetadataArtistName == aggArtist.Artist.Name)
            .ToListAsync();

        // i am thinking of returning a breakdown of timelines for this artist listeing, ie showiung x amount of listens in jan, feb. so i think i retunr every bit of possibel data and then the frointend can filter, so if user wants to view by year, they can, or month, or day, or whole time.

        return new ArtistTimeframeResponseDto()
        {
            TimeStamp = importedTracksForArtist.Select(x => x.TimeStamp).ToList(),
            PlayCount = importedTracksForArtist.Count(),
            RangeStart = input.rangeStart,
            RangeEnd = input.rangeEnd,
        };
    }

    public async Task<List<ArtistTrackResponseDto>> GetHistoricalArtistListens(Guid userId, Guid aggregateId)
    {
        var aggArtist = await _context.AggregatedArtists
            .Where(x => x.Artist.Id == aggregateId && x.UserId == userId)
            .SingleOrDefaultAsync();

        if (aggArtist == null)
        {
            throw new ArgumentNullException(nameof(aggArtist));
        }

        var importedTracksForArtist = await _context.ImportedTracks
            .Where(x => x.UserId == userId && x.MasterMetadataArtistName == aggArtist.Artist.Name).Select(x => new ArtistTrackResponseDto
            {
                MasterMetadataAlbumName = x.MasterMetadataAlbumName,
                MasterMetadataArtistName = x.MasterMetadataArtistName,
                MasterMetadataTrackName = x.MasterMetadataTrackName,
                TimeStamp = x.TimeStamp,
                MsPlayed = x.MsPlayed,
            }).ToListAsync();

        return importedTracksForArtist;
    }

    //public async Task<> Get


}
