using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;

namespace WebApi.Services.Interfaces;

public interface IUserAggregateArtistService
{
    Task<List<AggregateArtistDto>> GetAggregateArtists(User user);

}
