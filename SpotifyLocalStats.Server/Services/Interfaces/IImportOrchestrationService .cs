namespace WebApi.Services.Interfaces
{
    public interface IImportOrchestrationService
    {
        Task<IImportedTrackService> OrchestrateImport(string jsonData);
        Task<IModelPopulationService> PopulateModels(); // needs to pass in ImportedTrackdat?
        Task<IAggreationService> AggregateModelData(); // need to pass in populated models?
    }
}
