using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

using WebApi.Services.Interfaces;
using WebApi.Controllers.DTO;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ImportTracksJsonController : BasApiController
{
    public readonly IImportOrchestrationService _importOrchestrationService;

    [HttpPost]
    // maybe create a user dto and not pass the entire user object, just user id? 
    public async Task<ActionResult<ImportTracksDTO>> ImportTracks(User user, string json)
    {
        try 
        {
            var serialized = await _importedTrackService.ValidateIncomingJson(json);
            var tracksFinal = await _importedTrackService.AssignUser(serialized, user);

            var result = await _importedTrackService.SaveTracksToDb(tracksFinal);

            return Ok(new ImportTracksDTO
            {
                Count = result,
                ImportedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
