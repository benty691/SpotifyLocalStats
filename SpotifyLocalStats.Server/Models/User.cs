using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SpotifyLocalStats.Server.Models;

public class User : BaseModel
{
    public User(string userName, string firstName)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        Images = new List<Image>();
    }

    public DateTime LastUpdatedAt { get; set; }
    public DateTime LastTimeUsed { get; set; }
    public string? Email { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Country { get; set; } // either let user fill in or auto detect from ip
    public string? Phone { get; set; }
    public bool? Auth { get; set; } // is user authenticated with spotify
    public string? SpotifyPermissions { get; set; } // maybe enum later
    public string? SpotifyId { get; set; } // users spotify id
    public string? SpotifyUri { get; set; } // users spotify profile url
    public string? SpotifyDisplayName { get; set; } // users spotify display name
    public string? SpotifyHref { get; set; } // users spotify href
    public ICollection<Image>? Images { get; set; }
    public bool? IsPremium { get; set; } // is user premium
    [Required]
    public bool HasImportedHistorical { get; set; } // has the user uploaded historical data
}
