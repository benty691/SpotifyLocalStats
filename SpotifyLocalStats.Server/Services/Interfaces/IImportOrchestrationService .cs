using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces
{
    public interface IImportOrchestrationService
    {
        Task<int> Orchestrate(string jsonData, User user);
        /*
        Task <IEnumerable<ImportedTrack>> ImportTracks(string json, User user); // or return number of records imported?
        Task<IModelPopulationService> PopulateModels(); // needs to pass in ImportedTrackdat?
        Task<IAggregationService> AggregateModelData(); // need to pass in populated models?
        */
    }
}
