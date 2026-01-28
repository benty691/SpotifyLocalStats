using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SpotifyLocalStats.Server.Data;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ImportTracksJsonController : ControllerBase
{
    public readonly IImportedTrackService _importedTrackService;


    [HttpPost("ImportedTracks/{userId}")]

    public static SpotifyStatsContext _context;

    public async Task<ActionResult> ()
    {
        _context.Albums.AddRange();
    }
}
