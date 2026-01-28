using Microsoft.EntityFrameworkCore;

using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using WebApi.Controllers;
using WebApi.Services.Interfaces;


public class ImportedTrackService : IImportedTrackService
{

	private readonly SpotifyStatsContext _spotifyStatsContext;
	private readonly ILogger _logger;

    public ImportedTrackService(SpotifyStatsContext ctx, ILogger logger)
	{
		_spotifyStatsContext = ctx;
		_logger = logger;
    }


    public Task<IEnumerable<ImportedTrack>> ValidateIncomingJson(string json)
    {
        var importedTracks = JsonSerializer.Deserialize<IEnumerable<ImportedTrack>>(json);

        foreach (var track in importedTracks)
        {
            
        }
        
    }

    public Task HandleNullValues()
    {
        throw new NotImplementedException();
    }

    public Task SaveTracksToDb()
    {
        throw new NotImplementedException();
    }

    public Task<ImportedTrackService> DeserializeJson()
    {
        throw new NotImplementedException();
    }

    public 


}
