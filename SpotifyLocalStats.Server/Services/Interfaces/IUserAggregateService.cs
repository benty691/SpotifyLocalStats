using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs.NewFolder;

namespace WebApi.Services.Interfaces;

public interface IUserAggregateService<TAggregateDto> where TAggregateDto : AggregateBaseDto
{
    Task<List<TAggregateDto>> GetAggregate(User user, int pageNumber);
}
