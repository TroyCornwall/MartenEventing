using System.Threading.Tasks;

namespace PoCAPI.Services
{
    public class HeartbeatService
    {
        private readonly EventRaiser _eventRaiser;

        public HeartbeatService(EventRaiser eventRaiser)
        {
            _eventRaiser = eventRaiser;
        }

        public async Task CheckHeartbeat()
        {
            var seqId = await _eventRaiser.AddHeartBeat();
            //TODO: Poll database until event comes back
            

        }
    }
}