using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

using System;
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
        await SaveTracksToDb(updatedTrackList);

        return updatedTrackList;
    }

    public Task<IEnumerable<ImportedTrack>> ValidateIncomingJson(string json)
    {
        var importedTracks = JsonSerializer.Deserialize<IEnumerable<ImportedTrack>>(json);

        if (importedTracks == null)
        {
            _logger.LogError("Failed to deserialize incoming JSON to ImportedTrack collection.");
            throw new ArgumentNullException(nameof(importedTracks));
        }

        return Task.FromResult(importedTracks);
    }

    public IEnumerable<ImportedTrack> AssignUser(IEnumerable<ImportedTrack> importedTracks, User user) // user will come from controller
    {
        // we might want the user to sign in? or generate user for person on startup, so we have the user, can assign, rather than needing to generate on import?
        foreach (var track in importedTracks)
        {
            track.User = user; 
        }

        return importedTracks;
    }

    public async Task<int> SaveTracksToDb(IEnumerable<ImportedTrack> importedTracks)
    {
        await _spotifyStatsContext.ImportedTracks.AddRangeAsync(importedTracks);
        var numberOfRecordsSaved = await _spotifyStatsContext.SaveChangesAsync();
        var recordsSkipped = importedTracks.Count() - numberOfRecordsSaved;

        _logger.LogInformation($"Saved {numberOfRecordsSaved} imported tracks to database. {recordsSkipped} were skipped due to rule unique enforcment");
        // need to figure out how to handle if there is a duplicate imported track?? just skip and ignore, or handle and say these record has been uplaoded before?
        return numberOfRecordsSaved;
    }

    private string HashJsonContent(string json)
    {
        return GetHash(SHA256.Create(), json);
    }

    private string GetHash(HashAlgorithm hash, string input)
    {
        byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

}
