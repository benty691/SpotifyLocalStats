using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class StatsController : BaseApiController
{

    ILogger<StatsController> _logger;
    IUserBasicStatsService _userBasicStatsService;

    public StatsController(ILogger<StatsController> logger, IUserBasicStatsService userBasicStatsService)
    {
        _logger = logger;
        _userBasicStatsService = userBasicStatsService;
    }


    [HttpGet("/{userId}")]
    public async Task<ActionResult<UserSpotifyStatsDto>> GetUserSpotifyStatsBasic(Guid userId)
    {
        try
        {
            var userStats = await _userBasicStatsService.GetUserBasicStats(userId);

            return userStats == null ? StatusCode((int)HttpStatusCode.NotFound, $"User not found with id {userId}") : Ok(userStats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user stats with id: {userId}");
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Error retireiving user stats for id: {userId}");
        }
    }

    /*[HttpGet("/tracks/{userId}")]

    [HttpGet("/artists/{userId}")]

    [HttpGet("/albums/{userId}")]

    [HttpGet("/tracks/{userId}")]*/

}