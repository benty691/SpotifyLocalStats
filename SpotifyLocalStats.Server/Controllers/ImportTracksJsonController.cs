using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Text.Json;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ImportTracksJsonController : BaseApiController
{
    private readonly ILogger<ImportTracksJsonController> _logger;
    private readonly IImportOrchestrationService _importOrchestrationService;
    private readonly IUserService _userService;

    public ImportTracksJsonController(IImportOrchestrationService importOrchestrationService, ILogger<ImportTracksJsonController> logger, IUserService userService)
    {
        _importOrchestrationService = importOrchestrationService;
        _logger = logger;
        _userService = userService;
    }

    [HttpPost]
    // maybe create a user dto and not pass the entire user object, just user id? 
    public async Task<ActionResult<ImportTracksDTO>> ImportTracks([FromForm] string userId,  [FromForm] List<IFormFile> files)
    {
        if (!Guid.TryParse(userId, out var id)) {

            return BadRequest(new { error = "Invalid user ID format" });
        }

        var user = await _userService.GetUserById(id);

        if (user == null)
        {
            return NotFound(new { error = "User not found"});
        }

        if (files == null || !files.Any())
        {
            return BadRequest(new { error = "No files provided" });
        }

        foreach (var file in files)
        {
            try
            {
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                if (file.Length == 0)
                {
                    _logger.LogWarning($"file {file.Name} is empty, skipping file");
                    continue;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var result = await _importOrchestrationService.Orchestrate(json, user);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (JsonException ex)
            {
                return BadRequest(new { error = $"Invalid JSON in {file.FileName}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing tracks for user {user.Id}");
                return StatusCode(500, new { error = "An error occurred while importing tracks" });
            }
        }
        return Ok(new { message = $"Successfully processed {files.Count} file(s)" });
    }
}
