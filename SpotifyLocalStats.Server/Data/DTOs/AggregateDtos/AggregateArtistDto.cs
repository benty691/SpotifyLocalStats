using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs.NewFolder;

public class AggregateArtistDto : AggregateBaseDto
{
    public List<TimeOfDayStatDto<AggregatedArtist>> TimeOfDayStats { get; set; }
}
