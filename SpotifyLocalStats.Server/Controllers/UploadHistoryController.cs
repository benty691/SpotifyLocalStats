using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UploadHistoryController : BaseApiController
{
    private readonly ILogger<UploadHistoryController> _logger;
    private readonly IUploadHistoryService _uploadHistoryService;
    private readonly IUserService _userService;


    public UploadHistoryController(ILogger<UploadHistoryController> logger, IUploadHistoryService uploadHistoryService, IUserService userService)
    {
        _logger = logger;
        _uploadHistoryService = uploadHistoryService;
        _userService = userService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UploadHistoryResponseDto>> GetUploadHistory(string userId)
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
            var res = await _uploadHistoryService.GetUploadHistory(id);

            return res == null ? StatusCode((int)HttpStatusCode.NotFound, $"UploadHistory not found for {userId}") : Ok(res);
        }
        catch (Exception ex)
        {

            return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = $"Error returning upload history for user: {userId}", Error = ex });
        }
    }
}
