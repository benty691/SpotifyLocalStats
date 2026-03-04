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
                var newTracks = await _importedTrackService.HandleImport(json, user, file);
                job.ProgressPercent = 10;
                await _context.SaveChangesAsync();

                if (!newTracks.Any())
                {
                    job.Status = Models.Jobs.JobStatus.Duplicate;
                    job.CompletedAt = DateTime.UtcNow;
                    job.ErrorMessage = "All tracks in this file have already been imported.";
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ImportTracksDTO();
                }

                var result = await _modelPopulationService.PopulateModelsFromImportedTracks(newTracks);
                job.ProgressPercent = 55;
                await _context.SaveChangesAsync();

                await _aggreationService.UpdateAggregatedDataForUser(user, newTracks);
                job.ProgressPercent = 100;
                job.Status = Models.Jobs.JobStatus.Completed;
                job.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

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
