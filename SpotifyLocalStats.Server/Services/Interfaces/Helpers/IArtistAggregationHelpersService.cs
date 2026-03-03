namespace WebApi.Services.Interfaces.Helpers
{
    public interface IArtistAggregationHelpersService
    {
        Task RunCalculations(Guid userId);
    }
}
