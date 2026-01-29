using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces
{
    public interface IImportedTrackService
    {
        // essentially just want to ensure that the imported json is valid format, no null values where there shouldn't be, and then save to db
        Task<IEnumerable<ImportedTrack>> DeserializeJson(string json);
        Task<IEnumerable<ImportedTrack>> AssignPostSerializeValues(string importedTracks);
        Task SaveTracksToDb(IEnumerable<ImportedTrack> importedTracks);

    }
}
