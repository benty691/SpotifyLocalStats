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
        var nullArtistCount = 0;

        // ensure we deduplicate
        var uniqueImportedArtists = tracks
            .GroupBy(t => new { t.MasterMetadataArtistName })
            .Select(g => g.First())
            .ToList();

        var artistList = _context.Artists.ToDictionary(x => x.Name, x => x); // create a hashset we can lookup on for every track (O(n))

        // logic to generate artist from imported track
        foreach (var track in uniqueImportedArtists)
        {
            // for each track, we ideally try create an artist, if that artist already exists, we skip
            if (track.MasterMetadataArtistName == null)
            {
                _logger.LogDebug($"Artist name with trackId: {track.SpotifyTrackUri} is null");
                nullArtistCount++;
                continue;
            }
            // something to note is in json data we do not get spotify artsist url. My thinking is here we should query the webapi and try get it, so we can be definitive in artits, because artist names overlaps, i am sure. 
            if (!artistList.ContainsKey(track.MasterMetadataArtistName)) // O(1)
            {
                // we need spotify webapi to allow this to occur properly, as we neeed to hit the endpoint to get details, but we need the artist id from spotify to query??? 
                // appears we can use the search endpoint and search artist nam, and then get aristid from that, then query artist endpoint for details

                var newArtist = new Artist(track.MasterMetadataArtistName);
                await _context.Artists.AddAsync(newArtist);
                artistList.Add(track.MasterMetadataArtistName, newArtist);
            }
            else
            {
                _logger.LogWarning("Artist Already Exists, skipping");
                continue;
            }
        }

        var (artists, count) = await _context.SaveChangesWithResultAsync<Artist>();

        _logger.LogInformation($"Generated {count} new artists from imported tracks.\n {nullArtistCount} tracks with null artist.");

        return (artists, count);
    }

    private async Task<int> GenerateAlbum(IEnumerable<ImportedTrack> tracks)
    {
        var nullAlbumCount = 0;
        var uniqueAlbumList = tracks
            .GroupBy(x => new { x.MasterMetadataAlbumName })
            .Select(g => g.First())
            .ToList();

        var albumList = _context.Albums.ToDictionary(x => x.Name, x => x); //O(n)

        // var artistFromDb = await _context.Artists.ToListAsync();
        var artistList = _context.Artists.Local.ToList().ToDictionary(x => x.Name, x => x);

        // var combinedArtists = artistFromDb.Concat(artistList).ToDictionary(x => (x.Name), x => x);

        foreach (var track in uniqueAlbumList) // O(n)
        {
            if (track.MasterMetadataAlbumName == null)
            {
                _logger.LogDebug($"Album name with trackId: {track.SpotifyTrackUri} is null");
                nullAlbumCount++;
                continue;

            }

            if (!albumList.ContainsKey(track.MasterMetadataAlbumName))
            {
                if (artistList.TryGetValue(track.MasterMetadataArtistName, out var artist))
                {
                    var newAlbum = new Album(track.MasterMetadataAlbumName, artist);
                    await _context.Albums.AddAsync(newAlbum);
                    albumList.Add(track.MasterMetadataAlbumName, newAlbum);
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
        _logger.LogInformation($"Generated {result} new albums from imported tracks.\n {nullAlbumCount} tracks with null album.");
        return result;
    }

    private async Task<int> GenerateTrack(IEnumerable<ImportedTrack> tracks)
    {
        var nullTrackCount = 0;
        var uniqueTrackList = tracks.GroupBy(x => x.MasterMetadataTrackName).Select(g => g.First()).ToList();

        var trackList = _context.Tracks.ToDictionary((x => x.Name), x => x);

        //  var artistFromDb = await _context.Artists.ToListAsync();
        var artistList = _context.Artists.Local.ToList().ToDictionary(x => (x.Name), x => x);

        //  var combinedArtists = artistFromDb.Concat(artistList);

        //var albumsFromDB = await _context.Albums.ToListAsync();
        var albumList = _context.Albums.Local.ToList().ToDictionary(x => (x.Name), x => x);

        //var combinedAlbums = albumsFromDB.Concat(albumList).ToDictionary((x => x.Name), x => x);

        foreach (var track in uniqueTrackList)
        {
            if (track.MasterMetadataTrackName == null)
            {
                continue;
            }

            if (!trackList.ContainsKey(track.MasterMetadataTrackName))
            {
                if (!artistList.TryGetValue(track.MasterMetadataArtistName, out var artist))
                {
                    _logger.LogDebug($"No artist found when attempting to create track. Skipping track create for {track.MasterMetadataTrackName} and trackId: {track.SpotifyTrackUri}. Artist Name:{track.MasterMetadataArtistName}");

                    continue;
                }
                if (!albumList.TryGetValue(track.MasterMetadataAlbumName, out var album))
                {
                    _logger.LogDebug($"No Album found when attempting to create track. Skipping track create for {track.MasterMetadataTrackName} and trackId: {track.SpotifyTrackUri}. Album Name:{track.MasterMetadataAlbumName}");
                    continue;
                }

                var newTrack = new Track(artist, album, track.MasterMetadataTrackName, track.SpotifyTrackUri);
                await _context.Tracks.AddAsync(newTrack);
                trackList.Add(track.MasterMetadataTrackName, newTrack);
            }
            else
            {
                nullTrackCount++;
                _logger.LogWarning($"Track with ID {track.Id} does not have an album name.");
                continue;
            }
        }
        var result = await _context.SaveChangesAsync();
        _logger.LogInformation($"Generated {result} new albums from imported tracks.\n {nullTrackCount} tracks with null album.");
        return result;
    }
}