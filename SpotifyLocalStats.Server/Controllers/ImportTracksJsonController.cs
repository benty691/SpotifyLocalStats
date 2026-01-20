using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ImportTracksJsonController : Controller
    {
        [HttpPost("ImportedTracks/{userId}")]

        public async Task<ActionResult> ()
        {
            return View();
        }
    }
}
