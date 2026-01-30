using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces
{
    public interface IAggregationService
    {
        Task UpdateAggregatedDataForUser();

    }
}
