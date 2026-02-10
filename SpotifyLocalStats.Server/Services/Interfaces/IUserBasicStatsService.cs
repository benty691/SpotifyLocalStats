using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;

namespace WebApi.Services.Interfaces;

public interface IUserBasicStatsService
{
    Task<UserSpotifyStatsDto> GetUserBasicStats(Guid id);
}
