using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class UserAggregateArtistService : IUserAggregateArtistService
{

    public readonly SpotifyStatsContext _context;
    public readonly ILogger<UserAggregateArtistService> _logger;

    public UserAggregateArtistService(SpotifyStatsContext context, ILogger<UserAggregateArtistService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AggregateArtistDto>> GetAggregateArtists(User user)
    {
        var aggregateArtists = await _context.AggregatedArtists.Where(x => x.UserId == user.Id).Include(x => x.Artist).OrderByDescending(x => x.PlayCount).ToListAsync();
        var returnList = new List<AggregateArtistDto>();

        foreach (var aggregateArtist in aggregateArtists)
        {
            var newAggArtistRes = new AggregateArtistDto()
            {
                FirstListened = aggregateArtist.DateTimeFirstListened,
                LastListened = aggregateArtist.DateTimeLastListened,
                LongestDrySpell = aggregateArtist.LongestDrySpell,
                LongestDrySpellStart = aggregateArtist.LongestDrySpellStart,
                LongestDrySpellEnd = aggregateArtist.LongestDrySpellEnd,
                LongestStreak = aggregateArtist.LongestStreakDays,
                LongestStreakStart = aggregateArtist.LongestStreakStartDate,
                LongestStreakEnd = aggregateArtist.LongestStreakEndDate,
                MinsListened = aggregateArtist.MinsListened,
                MostTimesIn24Hours = aggregateArtist.MostTimesIn24Hours,
                Name = aggregateArtist.Artist.Name,
                PlayCount = aggregateArtist.PlayCount,
                TopListeningDate = aggregateArtist.TopListeningDate,
            };

            returnList.Add(newAggArtistRes);
        }

        return returnList;

    }
}
