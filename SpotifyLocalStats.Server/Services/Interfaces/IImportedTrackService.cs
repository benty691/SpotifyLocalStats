using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces
{
    public interface IImportedTrackService
    {
        // essentially just want to ensure that the imported json is valid format, no null values where there shouldn't be, and then save to db
        Task<IEnumerable<ImportedTrack>> HandleImport(string json, User user);
        Task<IEnumerable<ImportedTrack>> ValidateIncomingJson(string json);
        Task<IEnumerable<ImportedTrack>> AssignUser(IEnumerable<ImportedTrack> importedTracks, User user);
        Task<int> SaveTracksToDb(IEnumerable<ImportedTrack> importedTracks);

    }
}
