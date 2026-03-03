using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces;

public interface IUserArtistService
{
    Task<List<TAggregateDto>> GetAggregateArtists(User user);

}
