using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PoCAPI.Services;
using PoCCommon.Services;
using RestSharp;

namespace PoCAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly EventRaiser _eventRaiser;
        private readonly WatermarkService _watermarkService;

        public MessageController(ILogger<MessageController> logger, EventRaiser eventRaiser, WatermarkService watermarkService)
        {
            _logger = logger;
            _eventRaiser = eventRaiser;
            _watermarkService = watermarkService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            if (!message.StartsWith("H"))
            {
                return BadRequest("Message must start with a capital H");
            }
            if (!message.EndsWith("!"))
            {
                return BadRequest("Message must end with a !");
            }
            if (!message.Contains(" "))
            {
                return BadRequest("Message must contain a space");
            }

            var startTime = DateTime.Now;
            _logger.LogInformation($"Got message {message}");
            var watermark = _watermarkService.GetCurrentWatermark();
            var highestEventId = await _eventRaiser.AddMessage(message);
            
            //wait until watermark has been written to the db
            while (watermark.LastSequenceId < highestEventId)
            {
                watermark = _watermarkService.GetCurrentWatermark();
            }
            
            var endTime = DateTime.Now;
            var processingTime = endTime - startTime;
            
            
            return Ok($"Took {processingTime.TotalMilliseconds}ms to run");
        }
    }
}