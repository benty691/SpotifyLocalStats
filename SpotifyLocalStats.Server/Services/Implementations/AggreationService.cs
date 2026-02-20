using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
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

    public AggreationService(ILogger<AggreationService> logger, SpotifyStatsContext ctx, IArtistAggregationHelpersService artistAggregationHelpersService, IAlbumAggregationHelpersService albumAggregationHelpersService, ITrackAggregationHelpersService trackAggregationHelpersService)
    {
        _TrackAggregationHelpersService = trackAggregationHelpersService;
        _albumAggregationHelpersService = albumAggregationHelpersService;
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

            await _context.SaveChangesAsync();

            // should we have an orchestrator that calls all of these?
            await _artistAggregationHelpersService.RunCalculations();
            await _albumAggregationHelpersService.RunCalculations();
            await _TrackAggregationHelpersService.RunCalculations();

            await _context.SaveChangesAsync();

        // so in theolry here, we should have populated all the dbs now.. including calculated values, no idea how long this would take? fairly quick I would guess... 
        // then run 'baxkground' aggragtion helpers to fill in rest of the values... considering having this on a background service that updates daily when webapi opens (runs once after import of tracks, but for now, i think we just call after aggregates are created, essentially here. 
    }

    private async Task UpdateAggregateArtist(User user, IEnumerable<ImportedTrack> tracks)
    {
        var updatedCount = 0;
        var artistDict = _context.Artists.ToDictionary(x => x.Name, x => x);
        var aggArtistDict = _context.AggregatedArtists.Where(x => x.UserId == user.Id).Include(x => x.Artist.Name).ToDictionary(x => x.Artist.Name , x => x);
        var addedAggArtists = new Dictionary<string, AggregatedArtist>();


        // for each track that was upl;aoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach(var track in tracks) // o(n)
        {
            
            if(!artistDict.TryGetValue(track.MasterMetadataArtistName, out var artist))
            {

            }

            if(aggArtistDict.TryGetValue(track.MasterMetadataArtistName, out var aggregatedArtist))
            {
                continue;
            }
            if (aggArtistDict.TryGetValue(track.MasterMetadataArtistName, out var aggregated))
            {
                continue;
            }

            var aggregateArtists = _context.AggregatedArtists.Local.Where(x => artist.Name == track.MasterMetadataArtistName && x.User.Id == user.Id).ToList(); //O(2n^2) which is o(n^2)

            if (aggregateArtists.Count == 0)
            {
                var newAggArtist = new AggregatedArtist(artist)
                {
                    UniqueTracksPlayed = 1,
                    AlbumsListened = 1,
                    DateTimeFirstListened = track.TimeStamp,
                    DateTimeLastListened = track.TimeStamp,
                    User = user,
                    PlayCount = 1,
                    MsListened = track.MsPlayed,
                };

                await _context.AggregatedArtists.AddAsync(newAggArtist);
                aggArtistDict.Add(newAggArtist.Artist.Name, newAggArtist);

            }
            else if (aggregateArtists.Count == 1)
            {

                var aggregateArtist = aggregateArtists.Single();

                aggregateArtist.DateTimeLastListened = track.TimeStamp > aggregateArtist.DateTimeLastListened ? track.TimeStamp : aggregateArtist.DateTimeLastListened;
                aggregateArtist.PlayCount += 1;
                aggregateArtist.MsListened += track.MsPlayed;

                updatedCount++;
            }
            else
            {
                _logger.LogWarning("Multiple artists found with the same name, this is unhandable");
            }

            _logger.LogInformation($"Updated {updatedCount} records");
        }
    }

    private async Task UpdateAggregateAlbum(User user, IEnumerable<ImportedTrack> tracks)
    {
        var updatedCount = 0;


        // for each track that was uplaoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach (var track in tracks)
        {
            var album = await _context.Albums.Where(x => x.Name == track.MasterMetadataAlbumName).FirstOrDefaultAsync();

            if ( album is null)
            {
                throw new ArgumentNullException($"Album is null for album: {track.MasterMetadataAlbumName}");
            }

            var aggregatedAlbums = _context.AggregatedAlbums.Local.Where(x => album.Name == track.MasterMetadataAlbumName && x.User.Id == user.Id).ToList(); // done bby name which gets eh, we need ids 

            if (aggregatedAlbums.Count == 0)
            {
                // alot of these values will be calculater, either from wihtin the models get or via a background job???? 

                var newAggAlbum = new AggregatedAlbum(album)
                {
                    DateTimeFirstListened = track.TimeStamp,
                    DateTimeLastListened = track.TimeStamp,
                    User = user,
                    PlayCount = 1,
                    MsListened = track.MsPlayed,
                };

               await _context.AggregatedAlbums.AddAsync(newAggAlbum);
            }
            else if (aggregatedAlbums.Count == 1)
            {

                var aggregatedAlbum = aggregatedAlbums.Single();

                aggregatedAlbum.DateTimeLastListened = track.TimeStamp > aggregatedAlbum.DateTimeLastListened ? track.TimeStamp : aggregatedAlbum.DateTimeLastListened;
                aggregatedAlbum.PlayCount += 1;
                aggregatedAlbum.MsListened += track.MsPlayed;

                updatedCount++;
            }
            else
            {
                _logger.LogWarning("Multiple albums found with the same name, this is unhandable atm.");

            }
            _logger.LogInformation($"Updated {updatedCount} records");

        }
    }

    private async Task UpdateAggregateTrack(User user, IEnumerable<ImportedTrack> tracks)
    {
        var updatedCount = 0;

        // for each track that was uplaoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach (var track in tracks)
        {
            var trackLookup = await _context.Tracks.Where(x => x.Name == track.MasterMetadataTrackName).FirstAsync();

            var aggregatedTracks = _context.AggregatedTracks.Local.Where(x => trackLookup.SpotifyTrackUri == track.SpotifyTrackUri && x.User.Id == user.Id).ToList();

            if (aggregatedTracks.Count == 0)
            {
                var newAggTrack = new AggregatedTrack(trackLookup)
                {
                    DateTimeFirstListened = track.TimeStamp,
                    DateTimeLastListened = track.TimeStamp,
                    User = user,
                    PlayCount = 1,
                    MsListened = track.MsPlayed,
                };

                await _context.AggregatedTracks.AddAsync(newAggTrack);
            }
            else if (aggregatedTracks.Count == 1)
            {

                var aggregatedTrack = aggregatedTracks.Single();

                // we need to calcualte alot here. we should either delegate to functions, handle in the model that wokrs as a background service, or just do it here.

                aggregatedTrack.DateTimeLastListened = track.TimeStamp > aggregatedTrack.DateTimeLastListened ? track.TimeStamp : aggregatedTrack.DateTimeLastListened;
                aggregatedTrack.PlayCount += 1;
                aggregatedTrack.MsListened += track.MsPlayed;

                updatedCount++;
            }
            else
            {
                _logger.LogWarning("Multiple tracks found with the same name, this is unhandable atm.");

            }
        }
        _logger.LogInformation($"Updated {updatedCount} records");
    }
}
