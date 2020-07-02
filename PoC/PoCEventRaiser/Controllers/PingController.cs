using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PoCEventRaiser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get()
        {
            _logger.LogInformation($"Ping Request");
        }
    }
}