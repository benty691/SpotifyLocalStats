using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Implementations;

public class AggreationService
{
    private readonly ILogger<AggreationService> _logger;
    private readonly SpotifyStatsContext _context;
    public AggreationService(ILogger<AggreationService> logger, SpotifyStatsContext ctx)
    {
        _context = ctx;
        _logger = logger;
    }

    // do we just recalculate all aggregated data for a user?
    // or do we try and be smarter and just update what has changed?

    public async Task UpdateAggregatedDataForUser(User user, IEnumerable<ImportedTrack> tracks)
    {
        await UpdateAggregateArtist(user, tracks);
        await UpdateAggregateAlbum(user, tracks);
        await UpdateAggregateTrack(user, tracks);
    }

    private async Task UpdateAggregateArtist(User user, IEnumerable<ImportedTrack> tracks)
    {


        // for each track that was upl;aoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach(var track in tracks)
        {
            var aggregateArtists = _context.AggregatedArtists.Where(x => x.Artist.Name == track.MasterMetadataArtistName).ToList();

            if (aggregateArtists.Count == 0)
            {
                // alot of these values will be calculater, either from wihtin the models get or via a background job???? 

                var newAggArtist = new AggregatedArtist()
                {
                    Artist = _context.Artists.FirstOrDefault(x => x.Name == track.MasterMetadataArtistName),
                    UniqueTracksPlayed = 1,
                    AlbumsListened = 1,
                    TopTracks = _context.Tracks.Where(x => x.Name == track.MasterMetadataTrackName).ToList(),
                    TopAlbums = _context.Albums.Where(x => x.Name == track.MasterMetadataAlbumName).ToList(),
                    CreatedAt = DateTime.UtcNow,
                    DateTimeFirstListened = track.TimeStamp,
                    DateTimeLastListened = track.TimeStamp,
                    User = user,
                    PlayCount = 1,
                    MsListened = track.MsPlayed,
                };

                _context.AggregatedArtists.Add(newAggArtist);
            }
            else if (aggregateArtists.Count > 1)
            {
                _logger.LogWarning("Multiple artists found with the same name, this is unhandable atm.");
            }
            else
            {
                var aggregateArtist = aggregateArtists.First();

                // we need to calcualte alot here. we should either delegate to functions, handle in the model that wokrs as a background service, or just do it here.

                aggregateArtist.DateTimeLastListened = track.TimeStamp > aggregateArtist.DateTimeLastListened ? track.TimeStamp : aggregateArtist.DateTimeLastListened;
                aggregateArtist.PlayCount += 1;
                aggregateArtist.MsListened += track.MsPlayed;
                aggregateArtist.


                _logger.LogInformation("Artist already exists, updating stats not implemented yet.");
            }
        }

    }

}
