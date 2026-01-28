using Microsoft.EntityFrameworkCore;

using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using WebApi.Controllers;
using WebApi.Services.Interfaces;


public class ImportedTrackService : IImportedTrackService
{

	private readonly SpotifyStatsContext _spotifyStatsContext;
	private readonly ILogger _logger;
    private readonly User _user;

    public ImportedTrackService(SpotifyStatsContext ctx, ILogger logger)
	{
		_spotifyStatsContext = ctx;
		_logger = logger;
    }


    public Task<IEnumerable<ImportedTrack>> ValidateIncomingJson(string json)
    {
        var importedTracks = JsonSerializer.Deserialize<IEnumerable<ImportedTrack>>(json);

        if (importedTracks == null)
        {
            throw new ArgumentNullException(nameof(importedTracks));
        }

        return Task.FromResult(importedTracks);



        

    }

    public IEnumerable<ImportedTrack> AssignPostSerializeValues(IEnumerable<ImportedTrack> importedTracks)
    {
        // we might want the user to sign in? or generate user for person on startup, so we have the user, can assign, rather than needing to generate on import?
        var user = new User();

        foreach (var track in importedTracks)
        {
            track.ImportHash = HashJsonContent(JsonSerializer.Serialize<ImportedTrack>(track)); // setting hash content for each imported track (should enusre that no two exact same imported tracks). Stupid way to do? deserialise then reserialsie to do this? 

            track.User = _spotifyStatsContext.Users.Select(x => x.Id == user.Id).Single(); 
            // thinking here? cannot assign this to a user (table) where the user is yet to be created.. Need to create the user here? or will ef handle this for me?
        }
    }

    public Task HandleNullValues()
    {
        // not sure we need this? null values are ok. We just neeed to ensure the shape of data is correct but i think thats handled above

        throw new NotImplementedException();
    }

    public Task SaveTracksToDb()
    {
        // need to figure out how to handle if there is a duplicate imported track?? just skip and ignore, or handle and say these record has been uplaoded before?


        throw new NotImplementedException();
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
