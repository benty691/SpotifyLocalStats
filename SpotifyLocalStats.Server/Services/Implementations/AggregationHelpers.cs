using SpotifyLocalStats.Server.Data;

namespace WebApi.Services.Implementations;

public class ArtistAggregationHelpers
{
    private readonly ILogger<ArtistAggregationHelpers> _logger;
    private readonly SpotifyStatsContext _context;

    public ArtistAggregationHelpers(ILogger<ArtistAggregationHelpers> logger, SpotifyStatsContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<DateTime> CalculateTopListeningDate()
    {
        var aggArtists = _context.AggregatedArtists.ToList();

        if (aggArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        else
        {
            foreach (var aggArtist in aggArtists)
            {
                var importedTrack = _context.ImportedTracks.Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                    .ToList();

                if (importedTrack.Any())
                {
                    var topDay = importedTrack.GroupBy(x => x.TimeStamp.Date).Select(g => new
                    {
                        Date = g.Key,
                        PlayCount = g.Count()
                    })
                        .OrderByDescending(g => g.PlayCount)
                        .FirstOrDefault();

                    return topDay.Date;
                }
                else
                {
                    throw new InvalidOperationException($"No imported tracks found for artist {aggArtist.Artist.Name} and user {aggArtist.User.Id}.");
                }
            }

            throw new InvalidOperationException("No top listening date could be calculated.");
        }
    }

    public async Task<int> CalculateTotalPlayCount()
    {
        var aggArtists = _context.AggregatedArtists.ToList();
        int totalPlayCount= 0;

        if (aggArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in aggArtists)
        {
            totalPlayCount = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                .Count();
        }

        return totalPlayCount;
    }

    public async Task<int> CalculateUniqueTracksListened()
    {
        var aggArtists = _context.AggregatedArtists.ToList();
        int uniqueTracks = 0; 

        if (aggArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in aggArtists)
        {
            uniqueTracks = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                .Select(x => x.MasterMetadataTrackName)
                .Distinct()
                .Count();
        }
        return uniqueTracks;
    }

    public async Task<int> CalculateMostTimesIn24Hours()
    {
        // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)

        var aggArtists = _context.AggregatedArtists.ToList();
        int timesListend24Hours = 0;

        if (aggArtists.Count == 0)
        {
            throw new InvalidOperationException("No aggregated artists found.");
        }

        foreach (var aggArtist in aggArtists)
        {
            // need to get the max number of plays in any 24 hour period for this artist
            // set time frame from track time, then search 24 hours back, count numbver of times artist appears

            timesListend24Hours = _context.ImportedTracks
                .Where(x => x.MasterMetadataArtistName == aggArtist.Artist.Name && x.User.Id == aggArtist.User.Id)
                .
        }
        return uniqueAlbums;
    }
}
