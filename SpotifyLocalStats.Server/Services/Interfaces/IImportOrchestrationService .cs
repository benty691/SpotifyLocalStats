using SpotifyLocalStats.Server.Models;

using WebApi.Controllers.DTO;


namespace WebApi.Services.Interfaces
{
    public interface IImportOrchestrationService
    {
        Task<ImportTracksDTO> Orchestrate(string jsonData, User user);
        /*
        Task <IEnumerable<ImportedTrack>> ImportTracks(string json, User user); // or return number of records imported?
        Task<IModelPopulationService> PopulateModels(); // needs to pass in ImportedTrackdat?
        Task<IAggregationService> AggregateModelData(); // need to pass in populated models?
        */
    }
}
