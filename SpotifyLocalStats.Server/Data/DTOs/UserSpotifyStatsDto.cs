using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs;

// essentially the main user data passed likely to a user profile page?
public class UserSpotifyStatsDto
{
    public UserSpotifyStatsDto(int trackCount, int albumCount, int artistCount ) // then use gets to retriev that info??
    {
        TrackCount = trackCount;
        AlbumCount = albumCount;
        ArtistCount = artistCount;
    }
    public int TrackCount {get; set;} 
    public int ArtistCount {get; set;} 
    public int AlbumCount {get; set;}
}
