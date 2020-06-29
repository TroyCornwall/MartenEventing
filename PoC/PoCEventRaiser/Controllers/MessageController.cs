using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Commands.Events.PoC;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PoCEventRaiser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IDocumentStore _store;

        public MessageController(ILogger<MessageController> logger, IDocumentStore store)
        {
            _logger = logger;
            _store = store;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            _logger.LogInformation($"Got message {message}");
            EventStream es;
            using (var session = _store.OpenSession())
            {
                var events = new List<PocCharEvent>();
                foreach (var character in message)
                {
                    events.Add(new PocCharEvent()
                    {
                        Character = character
                    });
                }
                es = session.Events.StartStream(typeof(PocEvent), events);
                await session.SaveChangesAsync();
            }

            var seq = es.Events.Last().Sequence;
            _logger.LogInformation($"Last Sequence id - {seq}");
            return Ok(seq);
        }
    }
}