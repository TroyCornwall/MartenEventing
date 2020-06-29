using System.Collections.Generic;
using System.Linq;
using Marten;
using Marten.Events;

namespace PoCEventHandler.Services
{
    public class EventHandler
    {
        private IDocumentSession _session;
        
        public EventHandler(IDocumentSession session)
        {
            _session = session;
        }

        public List<IEvent> GetEvents(long currentPosition)
        {
            return _session.Events.QueryAllRawEvents()
                .Where(x => x.Sequence >= currentPosition)
                .OrderBy(x => x.Sequence)
                .ToList();
        }
    }
}