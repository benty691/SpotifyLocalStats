namespace WebApi.Services.Interfaces
{
    public interface IImportedTrackService
    {
        // essentially just want to ensure that the imported json is valid format, no null values where there shouldn't be, and then save to db
        Task<ImportedTrackService> DeserializeJson(string json);
        Task ValidateIncomingJson();
        Task HandleNullValues();
        Task SaveTracksToDb();

    }
}
