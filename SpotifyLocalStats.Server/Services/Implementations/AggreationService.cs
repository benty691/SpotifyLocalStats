using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Models;
using WebApi.Models.TimeOfDayConcretes;
using WebApi.Services.Interfaces;
using WebApi.Services.Interfaces.Helpers;

namespace WebApi.Services.Implementations;

public class AggreationService : IAggregationService
{
    private readonly ILogger<AggreationService> _logger;
    private readonly SpotifyStatsContext _context;
    private readonly IAggregationHelpersService<AggregatedArtist, ArtistTimeOfDayStat> _artistHelper;
    private readonly IAggregationHelpersService<AggregatedAlbum, AlbumTimeOfDayStat> _albumHelper;
    private readonly IAggregationHelpersService<AggregatedTrack, TrackTimeOfDayStat> _trackHelper;

    public AggreationService(
        ILogger<AggreationService> logger,
        SpotifyStatsContext ctx,
        IAggregationHelpersService<AggregatedArtist, ArtistTimeOfDayStat> artistHelper,
        IAggregationHelpersService<AggregatedAlbum, AlbumTimeOfDayStat> albumHelper,
        IAggregationHelpersService<AggregatedTrack, TrackTimeOfDayStat> trackHelper)
    {
        _context = ctx;
        _logger = logger;
        _artistHelper = artistHelper;
        _albumHelper = albumHelper;
        _trackHelper = trackHelper;
    }

    public async Task UpdateAggregatedDataForUser(User user, IEnumerable<ImportedTrack> tracks)
    {
        await UpdateAggregateArtist(user, tracks);
        await UpdateAggregateAlbum(user, tracks);
        await UpdateAggregateTrack(user, tracks);

        await _context.SaveChangesAsync();

        await _artistHelper.RunCalculations(user.Id);
        await _albumHelper.RunCalculations(user.Id);
        await _trackHelper.RunCalculations(user.Id);

        await _context.SaveChangesAsync();
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
            if (track.MasterMetadataArtistName == null)
            {
                throw new ArgumentException($"This is null somehow: {track.MasterMetadataTrackName}, {track.MasterMetadataAlbumName}, track id: {track.Id}");

            }

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

                    if (newAggArtist.Artist.Name == null)
                    {
                        throw new ArgumentException($"This is null somehow: {newAggArtist.Artist}, artist id: {newAggArtist.Artist.Id}");
                    }
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
                if (track.TimeStamp > artist.DateTimeLastListened)
                    artist.DateTimeLastListened = track.TimeStamp;
                if (track.TimeStamp < artist.DateTimeFirstListened)
                    artist.DateTimeFirstListened = track.TimeStamp;
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
                if (track.TimeStamp > album.DateTimeLastListened)
                    album.DateTimeLastListened = track.TimeStamp;
                if (track.TimeStamp < album.DateTimeFirstListened)
                    album.DateTimeFirstListened = track.TimeStamp;
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
                if (track.TimeStamp > trackValue.DateTimeLastListened)
                    trackValue.DateTimeLastListened = track.TimeStamp;
                if (track.TimeStamp < trackValue.DateTimeFirstListened)
                    trackValue.DateTimeFirstListened = track.TimeStamp;
                trackValue.PlayCount += 1;
            }
        }
        _logger.LogInformation($"Updated {updatedCount} records");
    }
}
