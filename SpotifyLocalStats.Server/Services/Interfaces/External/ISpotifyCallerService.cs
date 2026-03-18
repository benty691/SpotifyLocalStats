namespace WebApi.Services.Interfaces.External;

public interface ISpotifyCallerService
{
    Task ProcessAsync(string spotifyTrackId, CancellationToken cancellationToken);
}
