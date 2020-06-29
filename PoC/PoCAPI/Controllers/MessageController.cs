using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PoCAPI.Services;
using RestSharp;

namespace PoCAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly EventRaiser _eventRaiser;

        public MessageController(ILogger<MessageController> logger, EventRaiser eventRaiser)
        {
            _logger = logger;
            _eventRaiser = eventRaiser;
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
            var highestEventId = _eventRaiser.AddMessage(message);
           
            //TODO: Wait until event id is written back into DB

            var endTime = DateTime.Now;
            var processingTime = endTime - startTime;
            
            
            return Ok($"Took {processingTime} to run");
        }
    }
}