using SpotifyLocalStats.Server.Data;
using WebApi.Data.DTOs;

namespace WebApi.Controllers.DTO;

public class ImportTracksRequestDto
{
    public string Json { get; set; }
    public Guid UserId { get; set; }
}