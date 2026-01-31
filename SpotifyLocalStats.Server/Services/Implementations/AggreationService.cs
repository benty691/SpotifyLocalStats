using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Services.Implementations.Helpers;
using WebApi.Services.Interfaces;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations;

public class AggreationService : IAggregationService
{
    private readonly ILogger<AggreationService> _logger;
    private readonly SpotifyStatsContext _context;
    private readonly IArtistAggregationHelpersService _artistAggregationHelpersService;
    private readonly IAlbumAggregationHelpersService _albumAggregationHelpersService;
    private readonly ITrackAggregationHelpersService _TrackAggregationHelpersService;



    public AggreationService(ILogger<AggreationService> logger, SpotifyStatsContext ctx, IArtistAggregationHelpersService artistAggregationHelpersService)
    {
        _artistAggregationHelpersService = artistAggregationHelpersService;
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

        await _artistAggregationHelpersService.RunCalculations();
        await _albumAggregationHelpersService.RunCalculations();
        await 

        _context.SaveChanges();

        // then run 'baxkground' aggragtion helpers to fill in rest of the values... considering having this on a background service that updates daily when webapi opens (runs once after import of tracks, but for now, i think we just call after aggregates are created, essentially here. 
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
            else if (aggregateArtists.Count == 1)
            {

                var aggregateArtist = aggregateArtists.First();

                // we need to calcualte alot here. we should either delegate to functions, handle in the model that wokrs as a background service, or just do it here.

                aggregateArtist.DateTimeLastListened = track.TimeStamp > aggregateArtist.DateTimeLastListened ? track.TimeStamp : aggregateArtist.DateTimeLastListened;
                aggregateArtist.PlayCount += 1;
                aggregateArtist.MsListened += track.MsPlayed;

                _logger.LogInformation("Artist already exists, updating stats not implemented yet.");
            }
            else
            {
                _logger.LogWarning("Multiple artists found with the same name, this is unhandable atm.");

            }
        }

    }

}
