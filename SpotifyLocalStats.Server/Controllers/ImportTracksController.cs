using Microsoft.AspNetCore.Mvc;
using SpotifyLocalStats.Server.Data;
using WebApi.Data.DTOs;
using WebApi.Data.Jobs;
using WebApi.Models.Jobs;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ImportTracksController : BaseApiController
{
    private readonly ILogger<ImportTracksController> _logger;
    private readonly IUserService _userService;
    private readonly ImportJobQueue _queue;
    private readonly SpotifyStatsContext _context;

    public ImportTracksController(ILogger<ImportTracksController> logger, IUserService userService, ImportJobQueue jobQueue, SpotifyStatsContext spotifyStatsContext)
    {
        _logger = logger;
        _userService = userService;
        _queue = jobQueue;
        _context = spotifyStatsContext;
    }

    [HttpPost]
    [RequestSizeLimit(500_000_000)] // 500 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<List<ImportJobStatus>>> ImportTracks([FromForm] string userId, [FromForm] List<IFormFile> files)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            return BadRequest(new { error = "Invalid user ID format" });
        }

        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        if (files.Count == 0)
        {
            return BadRequest(new { error = "No files provided" });
        }

        var response = new List<ImportJobResponseDto>();

        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                _logger.LogWarning($"File {file.FileName} is empty, skipping");
                continue;
            }

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var job = _context.ImportJobStatuses.Add(new ImportJobStatus { Status = JobStatus.Queued });
            await _context.SaveChangesAsync();

            var jobId = job.Entity.Id;
            await _queue.EnqueAsync(new ImportJobData { JobId = jobId, File = file, Json = json, User = user });

            response.Add(
                new ImportJobResponseDto
                {
                    JobId = jobId,
                    StatusUrl = $"/ImportTracks/{jobId}/status"
                }
            );
        }
        return Accepted(response);
    }

    [HttpGet("{id}/status")]
    public IActionResult GetStatus(Guid id)
    {
        var job = _context.ImportJobStatuses.Find(id);
        if (job is null)
            return NotFound();

        return Ok(new ImportJobStatusDto
        {
            JobId = job.Id,
            Status = job.Status,
            ProgressPercent = job.ProgressPercent,
            ErrorMessage = job.ErrorMessage,
            CreatedAt = job.CreatedAt,
            CompletedAt = job.CompletedAt
        });
    }
}
