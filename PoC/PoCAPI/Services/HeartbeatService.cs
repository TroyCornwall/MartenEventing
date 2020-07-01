using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PoCCommon.Services;

namespace PoCAPI.Services
{
    public class HeartbeatService
    {
        private readonly EventRaiser _eventRaiser;
        private readonly WatermarkService _watermarkService;
        private readonly ILogger<HeartbeatService> _logger;

        public HeartbeatService(ILogger<HeartbeatService> logger, EventRaiser eventRaiser, WatermarkService watermarkService)
        {
            _eventRaiser = eventRaiser;
            _watermarkService = watermarkService;
            _logger = logger;
        }

        public async Task CheckHeartbeat()
        {
            _logger.LogInformation("Start heartbeat check");
            var startTime = DateTime.Now;
            var seqId = await _eventRaiser.AddHeartBeat();
            var waterLevel = 0L;
            while (waterLevel < seqId)
            {
                waterLevel = _watermarkService.GetCurrentWatermark().LastSequenceId;
            }
            
            var endTime = DateTime.Now;
            var processingTime = endTime - startTime;


           _logger.LogInformation($"Finished heartbeat  - {processingTime.TotalMilliseconds}ms");
            
        }
    }
}