using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class BasApiController : ControllerBase
    {
        private readonly ILogger<T> _logger;
        private readonly IConfiguration _configuration;
    }
}
