using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

using WebApi.Services.Interfaces;
using WebApi.Controllers.DTO;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ImportTracksJsonController : BaseApiController
{
    public readonly IImportOrchestrationService _importOrchestrationService;

    public ImportTracksJsonController(IImportOrchestrationService importOrchestrationService)
    {
        _importOrchestrationService = importOrchestrationService;
    }

    [HttpPost]
    // maybe create a user dto and not pass the entire user object, just user id? 
    public async Task<ActionResult<ImportTracksDTO>> ImportTracks(User user, string json)
    {
        try 
        {
            var serialized = await _importOrchestrationService.Orchestrate(json, user);

            return Ok(serialized);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
