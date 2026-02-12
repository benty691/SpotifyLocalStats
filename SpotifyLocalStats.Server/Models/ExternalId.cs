using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

public class ExternalId : BaseModel
{
    public string? Isrc { get; set; }
    public string? Ean { get; set; }
    public string? Upc { get; set; }
}
