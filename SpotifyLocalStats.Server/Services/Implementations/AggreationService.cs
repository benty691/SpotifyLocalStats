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
        // probs dont need
        var artistDict = _context.Artists.ToList().ToDictionary(x => x.Name, x => x);
        var aggArtistDict = _context.AggregatedArtists.Where(x => x.UserId == user.Id).Include(x => x.Artist).ToDictionary(x => x.Artist.Name, x => x);

        // for each track that was upl;aoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach (var track in tracks) // o(n)
        {
            if (!aggArtistDict.TryGetValue(track.MasterMetadataArtistName, out var artist))
            {
                var artistValue = artistDict.GetValueOrDefault(track.MasterMetadataArtistName);

                if (artistValue != null)
                {
                    var newAggArtist = new AggregatedArtist(artistValue)
                    {
                        UniqueTracksPlayed = 1,
                        AlbumsListened = 1,
                        DateTimeFirstListened = track.TimeStamp,
                        DateTimeLastListened = track.TimeStamp,
                        UserId = user.Id,
                        PlayCount = 1,
                        MsListened = track.MsPlayed,
                    };

                    await _context.AggregatedArtists.AddAsync(newAggArtist);
                    aggArtistDict.Add(newAggArtist.Artist.Name, newAggArtist);
                }
                else
                {
                    _logger.LogWarning($"cannot get artist from artist dictionary for artisdt name: {track.MasterMetadataArtistName}");
                }
            }

            else
            {
                artist.MsListened += track.MsPlayed;
                artist.DateTimeLastListened = track.TimeStamp;
                artist.PlayCount += 1;
            }
        }
        _logger.LogInformation($"Updated {updatedCount} records");
    }

    private async Task UpdateAggregateAlbum(User user, IEnumerable<ImportedTrack> tracks)
    {
        var updatedCount = 0;
        var albumDict = _context.Albums.ToDictionary(x => x.Name, x => x);
        var aggAlbumDict = _context.AggregatedAlbums.Where(x => x.UserId == user.Id).Include(x => x.Album).ToDictionary(x => x.Album.Name, x => x);

        // for each track that was upl;aoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach (var track in tracks) // o(n)
        {
            if (!aggAlbumDict.TryGetValue(track.MasterMetadataAlbumName, out var album))
            {
                var albumValue = albumDict.GetValueOrDefault(track.MasterMetadataAlbumName);

                if (albumValue != null)
                {
                    var newAggAlbum = new AggregatedAlbum(albumValue)
                    {
                        DateTimeFirstListened = track.TimeStamp,
                        DateTimeLastListened = track.TimeStamp,
                        UserId = user.Id,
                        PlayCount = 1,
                        MsListened = track.MsPlayed,
                    };

                    await _context.AggregatedAlbums.AddAsync(newAggAlbum);
                    aggAlbumDict.Add(newAggAlbum.Album.Name, newAggAlbum);
                }
                else
                {
                    _logger.LogWarning($"cannot get album from album dictionary for artist name: {track.MasterMetadataAlbumName}");
                }
            }

            else
            {
                album.MsListened += track.MsPlayed;
                album.DateTimeLastListened = track.TimeStamp;
                album.PlayCount += 1;
            }
        }
        _logger.LogInformation($"Updated {updatedCount} records");
    }

    private async Task UpdateAggregateTrack(User user, IEnumerable<ImportedTrack> tracks)
    {

        var updatedCount = 0;
        var trackDict = _context.Tracks.ToDictionary(x => x.Name, x => x);
        var aggTrackDict = _context.AggregatedTracks.Where(x => x.UserId == user.Id).Include(x => x.Track).ToDictionary(x => x.Track.Name, x => x);

        // for each track that was upl;aoded, we must check that trackj for the artist, if artist stats exist, increase count on things, esle create new agg stats 
        foreach (var track in tracks) // o(n)
        {
            if (!aggTrackDict.TryGetValue(track.MasterMetadataTrackName, out var trackValue))
            {
                var trackValueLookup = trackDict.GetValueOrDefault(track.MasterMetadataTrackName);

                if (trackValueLookup != null)
                {
                    var newAggTrack = new AggregatedTrack(trackValueLookup)
                    {
                        DateTimeFirstListened = track.TimeStamp,
                        DateTimeLastListened = track.TimeStamp,
                        UserId = user.Id,
                        PlayCount = 1,
                        MsListened = track.MsPlayed,
                    };

                    await _context.AggregatedTracks.AddAsync(newAggTrack);
                    aggTrackDict.Add(newAggTrack.Track.Name, newAggTrack);
                }
                else
                {
                    _logger.LogWarning($"cannot get album from album dictionary for artist name: {track.MasterMetadataAlbumName}");
                }
            }

            else
            {
                trackValue.MsListened += track.MsPlayed;
                trackValue.DateTimeLastListened = track.TimeStamp;
                trackValue.PlayCount += 1;
            }
        }
        _logger.LogInformation($"Updated {updatedCount} records");
    }
}
