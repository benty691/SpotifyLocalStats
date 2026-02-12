using SpotifyLocalStats.Server.Data;

namespace WebApi.Data.DTOs;

public class ImportTracksRequestDto
{
    public string Json { get; set; }
    public Guid UserId { get; set; }
}