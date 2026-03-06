using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;

namespace WebApi.Services.Interfaces;

public interface IModelPopulationService
{
    Task<ImportTrackResponseDto> PopulateModelsFromImportedTracks(IEnumerable<ImportedTrack> tracks);
    /*
    Task GenerateArtist(IEnumerable<ImportedTrack> tracks);
    Task GenerateAlbum(IEnumerable<ImportedTrack> tracks);
    Task GenerateTrack(IEnumerable<ImportedTrack> tracks);
    */
}
