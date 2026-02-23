using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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

    public async Task<IEnumerable<ImportedTrack>> HandleImport(string json, User user)
    {
        var trackList = await ValidateIncomingJson(json);
        var updatedTrackList = AssignUser(trackList, user);
        await SaveTracksToDb(updatedTrackList, user);

        return updatedTrackList;
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

        return importedTracks;
    }

    public IEnumerable<ImportedTrack> AssignUser(IEnumerable<ImportedTrack> importedTracks, User user) // user will come from controller
    {
        // we might want the user to sign in? or generate user for person on startup, so we have the user, can assign, rather than needing to generate on import?
        foreach (var track in importedTracks)
        {
            track.UserId = user.Id; 
        }

        return importedTracks;
    }

    public async Task<int> SaveTracksToDb(IEnumerable<ImportedTrack> importedTracks, User user)
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
        if (_spotifyStatsContext.ImportedTracks.Count() == 0)
        {
            await _spotifyStatsContext.ImportedTracks.AddRangeAsync(uniqueImportedTracks);
            var newTracksImported = await _spotifyStatsContext.SaveChangesAsync();
            return newTracksImported;
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
        }

        var numberOfRecordsSaved = await _spotifyStatsContext.SaveChangesAsync();
        var recordsSkipped = importedTracks.Count() - numberOfRecordsSaved;

        _logger.LogInformation($"Saved {numberOfRecordsSaved} imported tracks to database. {recordsSkipped} were skipped due to rule unique enforcment");
        return numberOfRecordsSaved;
    }
}
