using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Threading.Tasks;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class UserBasicStatsService : IUserBasicStatsService
{
    private readonly SpotifyStatsContext _context;
    public UserBasicStatsService (SpotifyStatsContext context)
    {
        _context = context ?? throw new ArgumentNullException (nameof (context));
    }

    public async Task<UserSpotifyStatsDto> GetUserBasicStats(Guid id)
    {
        int trackCount = await GetTrackStats(id);
        int albumCount = await GetAlbumStats(id);
        int artistCount = await GetArtistStats(id);

        return new UserSpotifyStatsDto(trackCount, albumCount, artistCount);
    }

    private async Task<int> GetTrackStats(Guid id)
    {
        var userTacks = await _context.AggregatedTracks.Where(x => x.UserId == id).ToListAsync();
        var count = 0;

        foreach (var track in userTacks)
        {
            count += track.PlayCount;
        }
        return count;
    }

    private async Task<int> GetAlbumStats(Guid id)
    {
        var userAlbums = await _context.AggregatedAlbums.Where(x => x.UserId == id).ToListAsync();
        var count = 0;

        foreach (var album in userAlbums)
        {
            count += album.PlayCount;
        }
        return count;
    }

    private async Task<int> GetArtistStats(Guid id)
    {
        var userArtists = await _context.AggregatedArtists.Where(x => x.UserId == id).ToListAsync();
        var count = 0;

        foreach (var artist in userArtists)
        {
            count += artist.PlayCount;
        }
        return count;
    }
}
