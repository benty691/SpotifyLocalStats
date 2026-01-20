using Microsoft.EntityFrameworkCore;

namespace SpotifyLocalStats.Server.Models;

public class CopyrightContent
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public string Type { get; set; } // C = Copyright, P = Performance
}
