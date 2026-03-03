using SpotifyLocalStats.Server.Models;

namespace WebApi.Data.DTOs.NewFolder
{
    public class AggregateTrackDto : AggregateBaseDto
    {
        public List<TimeOfDayStatDto<AggregatedTrack>> TimeOfDayStats { get; set; }
    }
}
