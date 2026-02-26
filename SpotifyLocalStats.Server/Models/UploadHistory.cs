using SpotifyLocalStats.Server.Models;

namespace WebApi.Models;

public class UploadHistory : BaseModel
{
    public string FileName { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
    public long? FileSize { get; set; } // I am thinknin g about stroiung data sizes in bytes?
}
