using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class UserBasicStatsService : IUserBasicStatsService
{
    private readonly SpotifyStatsContext _context;
    public UserBasicStatsService(SpotifyStatsContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserSpotifyStatsDto> GetUserBasicStats(Guid id)
    {
        int trackCount = await GetUniqueTracksCount(id);
        int albumCount = await GetUniqueAlbumCount(id);
        int artistCount = await GetUniqueArtistCount(id);

        return new UserSpotifyStatsDto(trackCount, albumCount, artistCount);
    }

    private async Task<int> GetUniqueTracksCount(Guid id)
    {
        return await _context.ImportedTracks.Where(x => x.UserId == id).CountAsync();
    }

    private async Task<int> GetUniqueAlbumCount(Guid id)
    {
        // total unique artists listend to 
        return await _context.AggregatedAlbums.Where(x => x.UserId == id).CountAsync();
    }

    private async Task<int> GetUniqueArtistCount(Guid id)
    {
        return await _context.AggregatedArtists.Where(x => x.UserId == id).CountAsync();
    }
}
