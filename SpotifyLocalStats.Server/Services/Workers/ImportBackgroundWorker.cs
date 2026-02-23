using SpotifyLocalStats.Server.Data;
using WebApi.Data.Jobs;
using WebApi.Models.Jobs;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Workers;

public class ImportBackgroundWorker : BackgroundService
{
    private readonly ImportJobQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImportBackgroundWorker> _logger;

    public ImportBackgroundWorker(ImportJobQueue queue, IServiceScopeFactory serviceScopeFactory, ILogger<ImportBackgroundWorker> logger)
    {
        _queue = queue;
        _scopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var data in _queue.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SpotifyStatsContext>();
            var importService = scope.ServiceProvider.GetRequiredService<IImportOrchestrationService>();

            var importJob = await context.ImportJobStatuses.FindAsync(data.JobId);
            importJob.Status = JobStatus.Processing;
            await context.SaveChangesAsync();

            try
            {
                await importService.ProcessAsync(data.Json, data.User, data.JobId, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import Job failed for id: {JobId}", data.JobId);
                importJob.Status = JobStatus.Failed;
                importJob.ErrorMessage = ex.Message;
                await context.SaveChangesAsync();
            }
        }
    }
}
