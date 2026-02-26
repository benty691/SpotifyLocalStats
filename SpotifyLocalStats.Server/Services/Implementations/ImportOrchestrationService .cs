using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
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

        public async Task ProcessAsync(string json, IFormFile file, User user, Guid jobId, CancellationToken cancellationToken)
        {
            await Orchestrate(json, file, user, jobId, cancellationToken);
        }

        private async Task<ImportTracksDTO> Orchestrate(string json, IFormFile file, User user, Guid jobId, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var job = _context.ImportJobStatuses.Find(jobId);

            try
            {
                var trackList = await _importedTrackService.HandleImport(json, user, file);
                job.ProgressPercent = 10;
                _context.ImportJobStatuses.Update(job);
                await _context.SaveChangesAsync();

                var result = await _modelPopulationService.PopulateModelsFromImportedTracks(trackList);
                job.ProgressPercent = 55;
                _context.ImportJobStatuses.Update(job);
                await _context.SaveChangesAsync();

                await _aggreationService.UpdateAggregatedDataForUser(user, trackList);
                job.ProgressPercent = 100;
                _context.ImportJobStatuses.Update(job);
                await _context.SaveChangesAsync();

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
