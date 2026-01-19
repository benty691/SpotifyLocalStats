using Microsoft.EntityFrameworkCore;
using System.Net.Cache;

namespace SpotifyLocalStats.Server.Models;

public class Artist
{
    public Guid Id { get; set; }
    public string SpotifyId { get; set; }
    public string Name { get; set; }
    public string SpotifyUrl { get; set; }
    public string Href { get; set; }
    public string Genres { get; set; } // maybe List<Genres> later
    public int TimesPLayed { get; set; } // total times played from tracks 'TimePlayed' field...
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateOnly DOB { get; set; } // Date of Birth
    public int Age 
    { 
        get 
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DOB.Year;
            if (DOB > today.AddYears(-age)) 
                age--;
            return age;
        }
    }
}

public class ArtistContext : DbContext
{
    public DbSet<Artist> Artists { get; set; }
}
