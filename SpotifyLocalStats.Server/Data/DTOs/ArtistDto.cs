using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs
{
    public class ArtistDto
    {
        public string Name { get; set; }
        public ICollection<Album> Albums { get; set; }
        public int? TimesPlayed { get; set; } // total times played from tracks 'TimePlayed' field...
    }
}
