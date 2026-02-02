using SpotifyLocalStats.Server.Models;
using WebApi.Controllers.DTO;

namespace WebApi.Services.Interfaces;

public interface IModelPopulationService
{
    Task<ImportTracksDTO> PopulateModelsFromImportedTracks(IEnumerable<ImportedTrack> tracks);
    /*
    Task GenerateArtist(IEnumerable<ImportedTrack> tracks);
    Task GenerateAlbum(IEnumerable<ImportedTrack> tracks);
    Task GenerateTrack(IEnumerable<ImportedTrack> tracks);
    */
}
