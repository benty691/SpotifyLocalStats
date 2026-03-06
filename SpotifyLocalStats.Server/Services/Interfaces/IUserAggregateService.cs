using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs.AggregateDtos;
using WebApi.Data.DTOs.NewFolder;

namespace WebApi.Services.Interfaces;

public interface IUserAggregateService<TAggregateDto> where TAggregateDto : AggregateBaseDto
{
    Task<AggregateResponseDto<TAggregateDto>> GetAggregate(User user, int pageNumber);
}
