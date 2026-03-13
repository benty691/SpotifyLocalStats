
namespace WebApi.Services.Auth
{
    public class SpotifyTokenProviderService : ISpotifyTokenProviderService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<SpotifyTokenProviderService> _logger;
        private readonly IHttpClientBuilder _httpClientBuilder;

        private readonly int TOKEN_REFRESH_TIME_INTERVAL = 600000; // ms one hour

        public SpotifyTokenProviderService(IConfiguration configuration, ILogger<SpotifyTokenProviderService> logger, IHttpClientBuilder httpClientBuilder)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientBuilder = httpClientBuilder;
        }

        public async Task<bool> TokenLifeCycleManager()
        {
            // essentially we need to keep track of if the token needs a refresh. so we just treat this the same as reading from a db, and if in needs a refresh, return true,l then th consumer of this will get a new token. 
            using (var timer = new Timer(
                callback: TimerTask,
                state: null,
                dueTime: 100,
                period: 3600000
                )
            {

            }


        }

        private void TimerTask(object? state)
        {
            // well this would probbaly generate a new token, then in that case we would not have to check if we need one, we are just always creating one? issues is that we would be making api calls that arent required, we only really need it when imports occur, no other time. 
        }










    }
}
}
