using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs;

// essentially the main user data passed likely to a user profile page?
public class UserSpotifyStatsDto
{
    public UserSpotifyStatsDto(Guid userId, int trackCount, int albumCount, int artistCount ) // then use gets to retriev that info??
    {
        UserId = userId; 
        TrackCount = trackCount;
        AlbumsCount = albumCount;
        ArtistCount = artistCount;
    }
    public Guid UserId {get; set;}
    public int TrackCount {get; set;} 
    public int ArtistCount {get; set;} 
    public int AlbumsCount {get; set;}
}
