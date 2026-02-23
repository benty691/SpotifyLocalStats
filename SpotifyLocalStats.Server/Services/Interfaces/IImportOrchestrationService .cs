using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;


namespace WebApi.Services.Interfaces
{
    public interface IImportOrchestrationService
    {
        Task ProcessAsync(string jsonData, User user, Guid jobId, CancellationToken cancellationToken);
        //Task<ImportTracksDTO> Orchestrate(string jsonData, User user, Guid jobId, CancellationToken cancellationToken);
        /*
        Task <IEnumerable<ImportedTrack>> ImportTracks(string json, User user); // or return number of records imported?
        Task<IModelPopulationService> PopulateModels(); // needs to pass in ImportedTrackdat?
        Task<IAggregationService> AggregateModelData(); // need to pass in populated models?
        */
    }
}
