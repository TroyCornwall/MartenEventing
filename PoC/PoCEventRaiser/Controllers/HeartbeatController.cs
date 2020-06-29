using System.Threading.Tasks;
using Common.Commands.Events;
using Common.Commands.Events.PoC;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PoCEventRaiser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;
        private readonly IDocumentStore _store;

        public HeartbeatController(ILogger<HeartbeatController> logger, IDocumentStore store)
        {
            _logger = logger;
            _store = store;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var heartBeat = new Heartbeat()
            {
                Source = "API Request",
                CreatedBy = "PoCEventRaiser"
            };

            using (var session = _store.OpenSession())
            {

                // Start a brand new stream and commit the new events as
                // part of a transaction
                // no stream type will be stored in database
                session.Events.StartStream(typeof(PocEvent), heartBeat);
                await session.SaveChangesAsync();
            } 
            
            
            // _session.Store(heartBeat);
            // await _session.SaveChangesAsync();
            
            return Ok(heartBeat);
        }
    }
}