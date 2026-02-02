using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Diagnostics.Metrics;
using WebApi.Controllers.DTO;
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

    public async Task<ImportTracksDTO> PopulateModelsFromImportedTracks(IEnumerable<ImportedTrack> tracks)
    {
        var aritstCount = await GenerateArtist(tracks);
        var albumCount = await GenerateAlbum(tracks);
        var trackCount = await GenerateTrack(tracks);

        return new ImportTracksDTO() {AlbumCount = albumCount, ArtistCount = aritstCount, TrackCount = trackCount }
        ;
    }

    private async Task<int> GenerateArtist(IEnumerable<ImportedTrack> tracks)
    {
        var nullArtistCount = 0;

        // logic to generate artist model from imported track
        foreach (var track in tracks)
        {
            // for each track, we ideally try create an artist, if that artist already exists, we skip
            if (track.MasterMetadataArtistName != null)
            {
                // something to note is in json data we do not get spotify artsist url. My thinking is here we should query the webapi and try get it, so we can be definitive in artits, because artist names overlaps, i am sure. 
                if (_context.Artists.Select(x => x.Name == track.MasterMetadataArtistName).Single())
                {
                    continue;
                }
                else
                {
                    // we need spotify webapi to allow this to occur properly, as we neeed to hit the endpoint to get details, but we need the artist id from spotify to query??? 
                    // appears we can use the search endpoint and search artist nam, and then get aristid from that, then query artist endpoint for details

                    await _context.Artists.AddAsync(new Artist
                    {
                        Name = track.MasterMetadataArtistName
                    });
                }
            }
            else
            {
                nullArtistCount++;
                _logger.LogWarning("Track with ID {TrackId} does not have an artist name.", track.Id);
                continue;
            }
        }
        var result = _context.SaveChanges();
        _logger.LogInformation($"Generated {result} new artists from imported tracks.\n {nullArtistCount} tracks with null artist.");

        return result; ;
    }

    private async Task<int> GenerateAlbum(IEnumerable<ImportedTrack> tracks)
    {
        var nullAlbumCount = 0;

        foreach (var track in tracks)
        {
            if (track.MasterMetadataAlbumName != null)
            {
                if (_context.Albums.Select(x => x.Name == track.MasterMetadataAlbumName).Single())
                {
                    continue;
                }
                else
                {
                    await _context.Albums.AddAsync(new Album
                    {
                        Name = track.MasterMetadataAlbumName,
                        Artists = _context.Artists.Where(a => a.Name == track.MasterMetadataArtistName).ToList()
                    });
                }
            }
            else
            {
                nullAlbumCount++;
                _logger.LogWarning("Track with ID {TrackId} does not have an album name.", track.Id);
                continue;
            }
        }
        var result = _context.SaveChanges();
        _logger.LogInformation($"Generated {result} new albums from imported tracks.\n {nullAlbumCount} tracks with null album.");
        return result;
    }

    private async Task<int> GenerateTrack(IEnumerable<ImportedTrack> tracks)
    {
        var nullTrackCount = 0;

        foreach (var track in tracks)
        {
            if (track.MasterMetadataTrackName != null)
            {
                if (_context.Albums.Select(x => x.Name == track.MasterMetadataAlbumName).Single())
                {
                    continue;
                }
                else
                {
                    await _context.Tracks.AddAsync(new Track
                    {
                        Name = track.MasterMetadataTrackName,
                        Artists = _context.Artists.Where(a => a.Name == track.MasterMetadataArtistName).ToList(),
                        Album = _context.Albums.Where(a => a.Name == track.MasterMetadataAlbumName).ToList(),
                        SpotifyTrackUri = track.SpotifyTrackUri,
                        Duration = track.MsPlayed,
                    });
                }
            }
            else
            {
                nullTrackCount++;
                _logger.LogWarning("Track with ID {TrackId} does not have an album name.", track.Id);
                continue;
            }
        }
        var result = _context.SaveChanges();
        _logger.LogInformation($"Generated {result} new albums from imported tracks.\n {nullTrackCount} tracks with null album.");
        return result;
    }
}