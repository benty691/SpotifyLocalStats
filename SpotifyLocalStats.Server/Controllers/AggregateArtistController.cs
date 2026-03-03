using Microsoft.AspNetCore.Mvc;
using SpotifyLocalStats.Server.Data;
using System.Net;
using WebApi.Data.DTOs.NewFolder;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("/api/[controller]")]
public class AggregateArtistController : BaseApiController
{
    private readonly SpotifyStatsContext _context;
    private readonly ILogger<AggregateArtistController> _logger;
    private readonly IUserAggregateService<AggregateArtistDto> _userAggregateService;
    private readonly IUserService _userService;


    public AggregateArtistController(SpotifyStatsContext context, ILogger<AggregateArtistController> logger, IUserAggregateService<AggregateArtistDto> userAggregateService, IUserService userService)
    {
        _context = context;
        _logger = logger;
        _userAggregateService = userAggregateService;
        _userService = userService;
    }


    [HttpGet("{userId}")]
    public async Task<ActionResult<List<AggregateArtistDto>>> GetAggregateArtists(string userId)
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
            var aggArtists = await _userAggregateService.GetAggregate(user);

            return aggArtists == null ? StatusCode((int)HttpStatusCode.NotFound, $"User Aggregate Artist Stats not found for userId: {userId}") : Ok(aggArtists);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user AggregateArtist stats with id: {userId}");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"Error retireiving user aggregate arttist stats for id: {userId}", Error = ex });
        }
    }
}
