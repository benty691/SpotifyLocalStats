using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using WebApi.Data.DTOs;
using WebApi.Data.Jobs;
using WebApi.Models.Jobs;
using WebApi.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ImportTracksJsonController : BaseApiController
{
    private readonly ILogger<ImportTracksJsonController> _logger;
    private readonly IImportOrchestrationService _importOrchestrationService;
    private readonly IUserService _userService;
    private readonly ImportJobQueue _queue;
    private readonly SpotifyStatsContext _context;

    public ImportTracksJsonController(IImportOrchestrationService importOrchestrationService, ILogger<ImportTracksJsonController> logger, IUserService userService, ImportJobQueue jobQueue)
    {
        _importOrchestrationService = importOrchestrationService;
        _logger = logger;
        _userService = userService;
        _queue = jobQueue;
    }

    [HttpPost]
    public async Task<ActionResult<ImportTracksDTO>> ImportTracks([FromForm] string userId, [FromForm] IFormFile file)
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

        if (file == null)
        {
            return BadRequest(new { error = "No files provided" });
        }

        if (file.Length == 0)
        {
            _logger.LogWarning("File {FileName} is empty, skipping", file.FileName);
            return BadRequest(new { error = "No file Content" });
        }

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        var job = _context.ImportJobStatuses.Add(new ImportJobStatus { Status = JobStatus.Queued });
        await _context.SaveChangesAsync();

        var jobId = job.Entity.Id;
        await _queue.EnqueAsync(new ImportJobData { JobId = jobId, Json = json });

        return Accepted(new { jobId, statusUrl = $"/import/{jobId}/status" });
    }

    [HttpGet("{id}/status")]
    public IActionResult GetStatus(Guid id)
    {
        var job = _context.ImportJobStatuses.Find(id);
        if (job is null) 
            return NotFound();

        return Ok(new
        {
            job.Id,
            Status = job.Status.ToString(),
            job.ProgressPercent,
            job.ErrorMessage,
            job.CreatedAt,
            job.CompletedAt
        });
    }
}
