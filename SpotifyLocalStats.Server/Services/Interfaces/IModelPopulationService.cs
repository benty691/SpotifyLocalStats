using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces
{
    public interface IModelPopulationService
    {
        Task PopulateModelsFromImportedTracks(IEnumerable<ImportedTrack> tracks);
        /*
        Task GenerateArtist(IEnumerable<ImportedTrack> tracks);
        Task GenerateAlbum(IEnumerable<ImportedTrack> tracks);
        Task GenerateTrack(IEnumerable<ImportedTrack> tracks);
        */
    }
}
