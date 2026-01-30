using Microsoft.AspNetCore.Mvc;
using SpotifyLocalStats.Server.Data;

namespace WebApi.Controllers
{
    public class BasApiController : ControllerBase
    {
        protected SpotifyStatsContext _spotifyStatsContext => (SpotifyStatsContext)HttpContext.RequestServices.GetService(typeof(SpotifyStatsContext));
    }
}