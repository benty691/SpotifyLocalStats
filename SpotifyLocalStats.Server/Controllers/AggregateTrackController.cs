using Microsoft.AspNetCore.Mvc;
using SpotifyLocalStats.Server.Data;
using System.Net;
using WebApi.Data.DTOs.NewFolder;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("/api/[controller]")]
public class AggregateTrackController : BaseApiController
{
    private readonly SpotifyStatsContext _context;
    private readonly ILogger<AggregateTrackController> _logger;
    private readonly IUserAggregateService<AggregateTrackDto> _userAggregateService;
    private readonly IUserService _userService;


    public AggregateTrackController(SpotifyStatsContext context, ILogger<AggregateTrackController> logger, IUserAggregateService<AggregateTrackDto> userAggregateService, IUserService userService)
    {
        _context = context;
        _logger = logger;
        _userAggregateService = userAggregateService;
        _userService = userService;
    }


    [HttpGet("{userId}/{pageNumber}")]
    public async Task<ActionResult<List<AggregateTrackDto>>> GetAggregateAlbum(string userId, int pageNumber)
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

        try
        {
            var aggArtists = await _userAggregateService.GetAggregate(user, pageNumber);

            return (aggArtists == null) ? StatusCode((int)HttpStatusCode.NotFound, $"User Aggregate Album Stats not found for userId: {userId}") : Ok(aggArtists);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user AggregateTrack stats with id: {userId}");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"Error retireiving user aggregate track stats for id: {userId}", Error = ex });
        }
    }

}
