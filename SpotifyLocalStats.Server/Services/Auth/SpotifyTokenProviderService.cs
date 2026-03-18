using System.Text.Json;
using WebApi.Data.DTOs;

namespace WebApi.Services.Auth
{
    public class SpotifyTokenProviderService : ISpotifyTokenProviderService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<SpotifyTokenProviderService> _logger;
        private string _accessToken;
        private DateTime _expiresAt;

        public SpotifyTokenProviderService(IConfiguration configuration, ILogger<SpotifyTokenProviderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetToken()
        {
            // checks if we have a valid token, if so, return it, if not, create one then return it
            var token = _accessToken;

            if (token == null || _expiresAt > DateTime.Now)
            {
                var response = await GenerateAccessToken();
                _accessToken = response.AccessToken;
                _expiresAt = DateTime.Now.AddSeconds(response.ExpiresIn - 60); // add one minute buffer
            }

            return token;
        }

        private async Task<SpotifyAccessTokenResponseDto> GenerateAccessToken()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://accounts.spotify.com/api/token")
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic " + _configuration["client_id"].ToString() + ":" + _configuration["client_secret"].ToString());


            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStreamAsync();

                var spotifyAccessResponse = await JsonSerializer.DeserializeAsync<SpotifyAccessTokenResponseDto>(body);

                if (spotifyAccessResponse == null)
                {
                    throw new ArgumentNullException($"spotify retunrned an error: {body}");
                }

                return spotifyAccessResponse;
            }

        }
    }
}
