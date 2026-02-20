using SpotifyLocalStats.Server.Data;
using WebApi.Data.Jobs;
using WebApi.Models.Jobs;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Workers;

public class ImportBackgroundWorker : BackgroundService
{
    private readonly ImportJobQueue _queue;
    private readonly SpotifyStatsContext _context;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImportBackgroundWorker> _logger;

    public ImportBackgroundWorker(ImportJobQueue queue, SpotifyStatsContext spotifyStatsContext, IServiceScopeFactory serviceScopeFactory, ILogger<ImportBackgroundWorker> logger)
    {
        _queue = queue;
        _context = spotifyStatsContext;
        _scopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var data in _queue.ReadAllAsync(stoppingToken))
        {
           var importJob = _context.ImportJobStatuses.Find(data.JobId);
            importJob.Status = JobStatus.Processing;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var importService = scope.ServiceProvider.GetRequiredService<IImportOrchestrationService>();

                await importService.Orchestrate(data.Json, data.User, data.JobId, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Import Job failed for id: {data.JobId}");
                importJob.Status = JobStatus.Failed;
                importJob.ErrorMessage = ex.Message;

                await _context.SaveChangesAsync();
        }
    }


}
