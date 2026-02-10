using Microsoft.AspNetCore.Mvc;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server;
using WebApi.Services.Interfaces;
using WebApi.Data.DTOs;

namespace WebApi.Controllers;

[Route("/user")]
public class UserController : BaseApiController
{
    IUserService _userService;
    IUserBasicStatsService _userBasicStatsService;

    public UserController(IUserService userService, IUserBasicStatsService userBasicStatsService)
    {
        _userBasicStatsService = userBasicStatsService;
        _userService = userService;
    }

    [HttpGet("/{userId}")]
    // kind of useless, because we are trying to get the user form the id, but we dont know there id on the forntend. we need a way to determine their record without their id from the frontend??? Maybe on create of the user, we send the 
    public ActionResult<UserDto> GetUserById(Guid userId)
    {
        // call a sertvice, which has the context, get user, return to us in a user dto, or we get the user here, the build the dto from the user
        try
        {
            var user = _userService.GetUserById(userId);

            return Ok(new UserDto(userId, user.UserName));

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // not sure if this should be in here or in a seperate 'stats' controller. Leaving here for now. 
    [HttpGet("/stats/{userId}")]
    public async Task<ActionResult<UserSpotifyStatsDto>> GetUserSpotifyStatsBasic(Guid userId)
    {
        try
        {
            var userStats = await _userBasicStatsService.GetUserBasicStats(userId);
            return Ok(userStats);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}