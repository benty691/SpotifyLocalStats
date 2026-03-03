using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public sealed class ModelPopulationService : IModelPopulationService
{
    private readonly ILogger<ModelPopulationService> _logger;
    private readonly SpotifyStatsContext _context;

    public ModelPopulationService(ILogger<ModelPopulationService> logger, SpotifyStatsContext context)
    {
        _logger = logger;
        _context = context;
    }


    // we need to retunr a list of succesfulkly saved records from each tables, so maybe create a new class represnting this retunr type.. 
    public async Task<ImportTracksDTO> PopulateModelsFromImportedTracks(IEnumerable<ImportedTrack> tracks)
    {
        var artistCount = await GenerateArtist(tracks);
        var albumCount = await GenerateAlbum(tracks);
        var trackCount = await GenerateTrack(tracks);

        return new ImportTracksDTO() { AlbumCount = albumCount, ArtistCount = artistCount.Item2, TrackCount = trackCount };
    }


    private async Task<(List<Artist>, int)> GenerateArtist(IEnumerable<ImportedTrack> tracks)
    {
        // so, we forgott o handle epidoseds / vbideos / podacts, that arent a song. so we hvae to remove these from the data, then  should be sweet. can add functionality later?
        // ensure we deduplicate
        var uniqueImportedArtists = tracks
            .GroupBy(t => new { t.MasterMetadataArtistName })
            .Select(g => g.First())
            .ToList();

        var artistList = _context.Artists.ToDictionary(x => x.Name, x => x); // create a hashset we can lookup on for every track (O(n))

        // logic to generate artist from imported track
        foreach (var track in uniqueImportedArtists)
        {
            // something to note is in json data we do not get spotify artsist url. My thinking is here we should query the webapi and try get it, so we can be definitive in artits, because artist names overlaps, i am sure. 

            if (!artistList.ContainsKey(track.MasterMetadataArtistName!)) // O(1)
            {
                // we need spotify webapi to allow this to occur properly, as we neeed to hit the endpoint to get details, but we need the artist id from spotify to query??? 
                // appears we can use the search endpoint and search artist nam, and then get aristid from that, then query artist endpoint for details

                var newArtist = new Artist(track.MasterMetadataArtistName!);
                await _context.Artists.AddAsync(newArtist);
                artistList.Add(track.MasterMetadataArtistName!, newArtist);
            }
            else
            {
                _logger.LogWarning("Artist Already Exists, skipping");
                continue;
            }
        }

        var (artists, count) = await _context.SaveChangesWithResultAsync<Artist>();

        _logger.LogInformation($"Generated {count} new artists from imported tracks.\n .");

        return (artists, count);
    }

    private async Task<int> GenerateAlbum(IEnumerable<ImportedTrack> tracks)
    {
        var uniqueAlbumList = tracks
            .GroupBy(x => new { x.MasterMetadataAlbumName })
            .Select(g => g.First())
            .ToList();

        var albumList = _context.Albums.ToDictionary(x => x.Name, x => x); //O(n)
        var artistList = _context.Artists.Local.ToList().ToDictionary(x => x.Name, x => x);

        foreach (var track in uniqueAlbumList) // O(n)
        {
            if (!albumList.ContainsKey(track.MasterMetadataAlbumName!))
            {
                if (artistList.TryGetValue(track.MasterMetadataArtistName!, out var artist))
                {
                    var newAlbum = new Album(track.MasterMetadataAlbumName!, artist);
                    await _context.Albums.AddAsync(newAlbum);
                    albumList.Add(track.MasterMetadataAlbumName!, newAlbum);
                }
                else
                {
                    _logger.LogDebug($"No artist found when attempting to create album. Skipping album create for {track.MasterMetadataAlbumName} and trackId: {track.SpotifyTrackUri}. Artist Name:{track.MasterMetadataArtistName}");
                    continue;
                }
            }
            else
            {
                _logger.LogWarning($"Album already exists for Album: {track.MasterMetadataAlbumName}");
                continue;
            }
        }
        var result = await _context.SaveChangesAsync();
        _logger.LogInformation($"Generated {result} new artists from imported tracks.\n .");
        return result;
    }

    private async Task<int> GenerateTrack(IEnumerable<ImportedTrack> tracks)
    {
        var uniqueTrackList = tracks
            .GroupBy(x => x.MasterMetadataTrackName)
            .Select(g => g.First())
            .ToList();

        var trackList = _context.Tracks.ToDictionary((x => x.Name), x => x);
        var artistList = _context.Artists.Local.ToList().ToDictionary(x => (x.Name), x => x);
        var albumList = _context.Albums.Local.ToList().ToDictionary(x => (x.Name), x => x);

        foreach (var track in uniqueTrackList)
        {
            if (!trackList.ContainsKey(track.MasterMetadataTrackName!))
            {
                if (!artistList.TryGetValue(track.MasterMetadataArtistName!, out var artist))
                {
                    _logger.LogDebug($"No artist found when attempting to create track. Skipping track create for {track.MasterMetadataTrackName} and trackId: {track.SpotifyTrackUri}. Artist Name:{track.MasterMetadataArtistName}");

                    continue;
                }
                if (!albumList.TryGetValue(track.MasterMetadataAlbumName!, out var album)) // use ! as it cannot be null because of the .Where()
                {
                    _logger.LogDebug($"No Album found when attempting to create track. Skipping track create for {track.MasterMetadataTrackName} and trackId: {track.SpotifyTrackUri}. Album Name:{track.MasterMetadataAlbumName}");
                    continue;
                }

                var newTrack = new Track(
                    artist,
                    album,
                    track.MasterMetadataTrackName!,
                    track.SpotifyTrackUri
                    );
                await _context.Tracks.AddAsync(newTrack);
                trackList.Add(track.MasterMetadataTrackName!, newTrack);
            }
        }
        var result = await _context.SaveChangesAsync();
        _logger.LogInformation($"Generated {result} new albums from imported tracks.\n");
        return result;
    }
}