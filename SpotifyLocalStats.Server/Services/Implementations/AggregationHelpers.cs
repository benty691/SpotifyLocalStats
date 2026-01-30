using SpotifyLocalStats.Server.Data;

namespace WebApi.Services.Implementations;

public class AggregationHelpers
{
    private readonly ILogger<AggregationHelpers> _logger;
    private readonly SpotifyStatsContext _context;

    public AggregationHelpers(ILogger<AggregationHelpers> logger, SpotifyStatsContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task CalculateTopListeningDate()
    {
        var aggArtists = _context.AggregatedArtists.ToList();

        foreach(var aggArtist in aggArtists)
        {
            var importedTrack = _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                .ToList();

            if (importedTrack.Any())
            {

            }
        }


    }

}
