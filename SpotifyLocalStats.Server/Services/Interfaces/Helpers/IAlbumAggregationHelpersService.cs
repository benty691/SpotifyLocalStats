namespace WebApi.Services.Interfaces.Helpers
{
    public interface IAlbumAggregationHelpersService
    {
        Task RunCalculations(Guid userId);
    }
}
