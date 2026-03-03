using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs.NewFolder;

public class AggregateAlbumDto : AggregateBaseDto
{
    public List<TimeOfDayStatDto<AggregatedAlbum>> TimeOfDayStats { get; set; }

}
