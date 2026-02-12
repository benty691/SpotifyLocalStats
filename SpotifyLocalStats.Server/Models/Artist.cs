using Microsoft.EntityFrameworkCore;
using System.Net.Cache;

namespace SpotifyLocalStats.Server.Models;

public class Artist : BaseModel
{
    public Artist(string name)
    {
        Images = new List<Image>();
        Albums = new List<Album>();
        ExternalIds = new List<ExternalId>();
        Name = name ?? throw new ArgumentNullException(nameof(Name));
    }
    public string Name { get; set; }
    public string? SpotifyId { get; set; }
    public string? SpotifyUrl { get; set; }
    public ICollection<Image> Images { get; set; }
    public ICollection<Album> Albums { get; set; }
    public string? Href { get; set; }
    public string? Genres { get; set; } // maybe List<Genres> later
    public int? TimesPlayed { get; set; } // total times played from tracks 'TimePlayed' field...
    public ICollection<ExternalId> ExternalIds { get; set; }
    public bool? IsBand { get; set; } // is the artist a band or solo artist
    public DateOnly DOB { get; set; } // Date of Birth
    public int? Age 
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

