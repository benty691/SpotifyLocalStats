namespace WebApi.Services.Interfaces.Helpers
{
    public interface ITrackAggregationHelpersService
    {
        Task RunCalculations(Guid userId);
    }
}
