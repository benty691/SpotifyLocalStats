using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("/import")]
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
    public async Task<ActionResult<ImportTracksDTO>> ImportTracks([FromBody] ImportTracksRequestDto importTracksRequest)
    {
        try 
        {
            var user = await _userService.GetUserById(importTracksRequest.UserId);
            if (user == null) 
            {
                return NotFound(new { error = "User not found" });
            }

            var result = await _importOrchestrationService.Orchestrate(importTracksRequest.Json, user);
            return Ok(result);
        }
        catch (ArgumentException ex) 
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error importing tracks for user {importTracksRequest.UserId}");
            return StatusCode(500, new { error = "An error occurred while importing tracks" });
        }
    }
}
