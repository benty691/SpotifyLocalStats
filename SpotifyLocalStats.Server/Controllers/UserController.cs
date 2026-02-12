using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SpotifyLocalStats.Server;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Net;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("/user")]
public class UserController : BaseApiController
{
    ILogger<UserController> _logger;
    IUserService _userService;

    public UserController(IUserService userService, IUserBasicStatsService userBasicStatsService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("/{userId}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        // call a sertvice, which has the context, get user, return to us in a user dto, or we get the user here, the build the dto from the user
        try
        {
            var user = await _userService.GetUserById(userId);

            var userDto = new UserDto(user.Id, user.UserName);

            return user == null ? NotFound() : Ok(userDto);

        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user with id: {userId}");
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Error retireiving user with id: {userId}");

        }
    }

    // not sure if this should be in here or in a seperate 'stats' controller. Leaving here for now. 
 

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUser(request.UserName, request.UserFirstName);

            var userDto = new UserDto(user.Id, user.UserName);

            return CreatedAtAction(
                nameof(CreateUser),
                userDto
                );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating user with UserName:{request.UserName} and FirstName:{request.UserFirstName}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occured while creating the user.");
        }
    }
}