using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

public class Image
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string Height { get; set; }
    public string Width { get; set; }
}
