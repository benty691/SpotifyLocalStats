namespace SpotifyLocalStats.Server.Models;

public class Image : BaseModel
{
    public string? Url { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
}
