using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Diagnostics.Metrics;
using System.Linq;
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

    public async Task<ImportTracksDTO> PopulateModelsFromImportedTracks(IEnumerable<ImportedTrack> tracks)
    {
        var aritstCount = await GenerateArtist(tracks);
        var albumCount = await GenerateAlbum(tracks);
        var trackCount = await GenerateTrack(tracks);

        return new ImportTracksDTO() { AlbumCount = albumCount, ArtistCount = aritstCount, TrackCount = trackCount };
    }

    private async Task<int> GenerateArtist(IEnumerable<ImportedTrack> tracks)
    {
        var nullArtistCount = 0;
        // ok this is painfully slow. like minutes. ths is tererible. I think my solution currently is like 0(n^2^2^2) or some shit. 

       var artistList = _context.Artists.Local.Select(x => new {x.Name }).ToHashSet(); // create a hashset we can lookup on for every track 

        // logic to generate artist model from imported track
        foreach (var track in tracks) //O(n)
        {
            // for each track, we ideally try create an artist, if that artist already exists, we skip
            if (track.MasterMetadataArtistName != null)
            {
                // something to note is in json data we do not get spotify artsist url. My thinking is here we should query the webapi and try get it, so we can be definitive in artits, because artist names overlaps, i am sure. 

                // we are checking change tarcker instead of the db. Do this to avoid calling saveChanges after everytime we add an artistt
                if (!artistList.Contains(track.MasterMetadataArtistName)) // O(n^2)
                {
                    // we need spotify webapi to allow this to occur properly, as we neeed to hit the endpoint to get details, but we need the artist id from spotify to query??? 
                    // appears we can use the search endpoint and search artist nam, and then get aristid from that, then query artist endpoint for details

                    await _context.Artists.AddAsync(new Artist(track.MasterMetadataArtistName));
                }
                else
                {
                    continue;
                }
            }
            else
            {
                nullArtistCount++;
                _logger.LogWarning($"Track with ID {track.Id} does not have an artist name.");
                continue;
            }
        }
        var result = await _context.SaveChangesAsync();
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
                // we are checking change tarcker instead of the db. Do this to avoid calling saveChanges after everytime we add an Album
                if (_context.Albums.Local.Where(x => x.Name == track.MasterMetadataAlbumName).Count() > 0)
                {
                    continue;
                }
                else
                {
                    await _context.Albums.AddAsync(new Album(track.MasterMetadataAlbumName)
                    {
                        Artists = await _context.Artists.Where(a => a.Name == track.MasterMetadataArtistName).ToListAsync()
                    });
                }
            }
            else
            {
                nullAlbumCount++;
                _logger.LogWarning($"Track with ID {track.Id} does not have an album name.");
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

        foreach (var track in tracks)
        {
            if (track.MasterMetadataTrackName != null)
            {

                // we are checking change tarcker instead of the db. Do this to avoid calling saveChanges after everytime we add an track
                if (_context.Tracks.Local.Where(x => x.Name == track.MasterMetadataTrackName).Count() > 0)
                {
                    continue;
                }
                else
                {
                    var newTrack = new Track(track.MasterMetadataTrackName, track.SpotifyTrackUri)
                    {
                        Artists = await _context.Artists.Where(a => a.Name == track.MasterMetadataArtistName).ToListAsync(),
                        Albums = await _context.Albums.Where(a => a.Name == track.MasterMetadataAlbumName).ToListAsync()
                    };

                    await _context.Tracks.AddAsync(newTrack);
                }
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