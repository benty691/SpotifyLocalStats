using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using System.Text;
using System.Text.Json;
using WebApi.Models;
using WebApi.Services.Interfaces;


public class ImportedTrackService : IImportedTrackService
{
    private readonly SpotifyStatsContext _spotifyStatsContext;
    private readonly ILogger _logger;

    public ImportedTrackService(SpotifyStatsContext ctx, ILogger<ImportedTrackService> logger)
    {
        _spotifyStatsContext = ctx;
        _logger = logger;
    }

    public async Task<IEnumerable<ImportedTrack>> HandleImport(string json, User user, IFormFile file)
    {
        var trackList = await ValidateIncomingJson(json);
        var updatedTrackList = await AssignUserAndUpload(trackList, user, file);
        var newTracks = await SaveTracksToDb(updatedTrackList, user);

        return newTracks;
    }

    public async Task<IEnumerable<ImportedTrack>> ValidateIncomingJson(string json)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var importedTracks = await JsonSerializer.DeserializeAsync<IEnumerable<ImportedTrack>>(stream);

        if (importedTracks == null)
        {
            _logger.LogError("Failed to deserialize incoming JSON to ImportedTrack collection.");
            throw new ArgumentNullException(nameof(importedTracks));
        }

        importedTracks.Where(x => x.MasterMetadataArtistName != null && x.MasterMetadataArtistName != string.Empty).ToList();

        // removes null data from the beginning.
        return importedTracks
            .Where(x => x.MasterMetadataArtistName != null && x.MasterMetadataArtistName != string.Empty && x.MasterMetadataAlbumName != null && x.MasterMetadataAlbumName != string.Empty && x.MasterMetadataTrackName != null && x.MasterMetadataTrackName != string.Empty).OrderBy(x => x.TimeStamp)
            .ToList();
    }

    public async Task<IEnumerable<ImportedTrack>> AssignUserAndUpload(IEnumerable<ImportedTrack> importedTracks, User user, IFormFile file) // user will come from controller
    {
        // we might want the user to sign in? or generate user for person on startup, so we have the user, can assign, rather than needing to generate on import?
        var uploadHistory = new UploadHistory()
        {
            UserId = user.Id,
            FileName = file.FileName,
            FileSize = file.Headers.ContentLength
        };

        await _spotifyStatsContext.UploadHistories.AddAsync(uploadHistory);


        foreach (var track in importedTracks)
        {
            track.UserId = user.Id;
            track.UploadHistoryId = uploadHistory.Id;
        }

        return importedTracks;
    }

    public async Task<List<ImportedTrack>> SaveTracksToDb(IEnumerable<ImportedTrack> importedTracks, User user)
    {
        // De-duplicate incoming trackList
        var uniqueImportedTracks = importedTracks
            .GroupBy(t => new { t.TimeStamp, t.SpotifyTrackUri })
            .Select(g => g.First())
            .ToList();

        var duplicatesInImport = importedTracks.Count() - uniqueImportedTracks.Count;
        if (duplicatesInImport > 0)
        {
            _logger.LogInformation($"Found {duplicatesInImport} duplicates within the imported data");
        }

        // fresh import
        if (!_spotifyStatsContext.ImportedTracks.Any(x => x.UserId == user.Id))
        {
            await _spotifyStatsContext.ImportedTracks.AddRangeAsync(uniqueImportedTracks);
            await _spotifyStatsContext.SaveChangesAsync();
            return uniqueImportedTracks;
        }

        var existingKeys = _spotifyStatsContext.ImportedTracks
            .Where(x => x.UserId == user.Id)
            .Select(x => new { x.TimeStamp, x.SpotifyTrackUri })
            .ToHashSet();

        var newTracks = uniqueImportedTracks
            .Where(track => !existingKeys.Contains(new { track.TimeStamp, track.SpotifyTrackUri }))
            .ToList();

        if (newTracks.Any())
        {
            await _spotifyStatsContext.ImportedTracks.AddRangeAsync(newTracks);
            await _spotifyStatsContext.SaveChangesAsync();
        }

        _logger.LogInformation($"Saved {newTracks.Count} imported tracks to database. {uniqueImportedTracks.Count - newTracks.Count} were skipped as duplicates");
        return newTracks;
    }
}
