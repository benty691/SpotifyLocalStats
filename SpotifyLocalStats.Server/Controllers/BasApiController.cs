using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class BasApiController : Controller
    {
        private readonly ILogger<T> _logger;
        private readonly IConfiguration _configuration;
    }
}
