namespace SpotifyLocalStats.Server.Models;

public abstract class BaseModel
{
    public BaseModel()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } 
}
