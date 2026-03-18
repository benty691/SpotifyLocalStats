namespace WebApi.Services.Auth;

public class SpotifyAuthHandler : DelegatingHandler
{
    private readonly ISpotifyTokenProviderService _spotifyTokenProviderService;

    public SpotifyAuthHandler(ISpotifyTokenProviderService spotifyTokenProviderService)
    {
        _spotifyTokenProviderService = spotifyTokenProviderService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _spotifyTokenProviderService.GetToken();

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
