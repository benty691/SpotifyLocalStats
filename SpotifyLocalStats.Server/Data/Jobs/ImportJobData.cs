using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.Jobs;

public class ImportJobData
{
    public Guid JobId { get; set; }
    public string Json { get; set; }
    public IFormFile File { get; set; }
    public User User { get; set; }
}
