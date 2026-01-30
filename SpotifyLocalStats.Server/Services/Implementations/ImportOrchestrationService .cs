using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Controllers.DTO;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations
{
    public class ImportOrchestrationService : IImportOrchestrationService
    {
        private readonly IImportedTrackService _importedTrackService;
        private readonly IAggregationService _aggreationService;
        private readonly IModelPopulationService _modelPopulationService;
        private readonly ILogger<ImportOrchestrationService> _logger;
        private readonly SpotifyStatsContext _context;

        public ImportOrchestrationService(IImportedTrackService importedTrackService, IModelPopulationService modelPopulationService, IAggregationService aggreationService, SpotifyStatsContext context)
        {
            _importedTrackService = importedTrackService;
            _modelPopulationService = modelPopulationService;
            _aggreationService = aggreationService;
            _context = context;
        }

        public async Task<int> Orchestrate(string json, User user)
        {
            var trackList = await _importedTrackService.HandleImport(json, user);
            
            await _modelPopulationService.PopulateModelsFromImportedTracks(trackList);
            await _aggreationService.UpdateAggregatedDataForUser(trackList, user);
            return result;
        }
    }
}
