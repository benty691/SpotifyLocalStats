using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs;

// essentially the main user data passed likely to a user profile page?
public class UserSpotifyStats
{

    public UserSpotifyStats(Guid userId) // then use gets to retriev that info??
    {

//        UserName = getUserName();

    }
    public UserDto User {get; set;}
    public int totalTracks {get; set;} // every single imported track count
    public int totalArtists {get; set;} // every single imported track count
    public int totalAlbums {get; set;}
}
