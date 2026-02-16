using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Transactions;
using WebApi.Data.DTOs;
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

        public ImportOrchestrationService(ILogger<ImportOrchestrationService> logger, IImportedTrackService importedTrackService, IModelPopulationService modelPopulationService, IAggregationService aggreationService, SpotifyStatsContext context)
        {
            _logger = logger;
            _importedTrackService = importedTrackService;
            _modelPopulationService = modelPopulationService;
            _aggreationService = aggreationService;
            _context = context;
        }

        public async Task<ImportTracksDTO> Orchestrate(string json, User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var trackList = await _importedTrackService.HandleImport(json, user);

                var result = await _modelPopulationService.PopulateModelsFromImportedTracks(trackList);
                await _aggreationService.UpdateAggregatedDataForUser(user, trackList);

                // return amount of records processed, few other smaller details, via a dto creation? 
                // maybe return loading until processing is finished?
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error: {ex.Message}");
                throw;
            }
            
        }
    }
}
