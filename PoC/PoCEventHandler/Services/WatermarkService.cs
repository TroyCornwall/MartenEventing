using System;
using System.Linq;
using PoCCommon.Database;
using PoCCommon.Database.Models;

namespace PoCEventHandler.Services
{
    public class WatermarkService
    {
        private readonly PocDbContext _dbContext;
        
        public WatermarkService(PocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public long GetCurrentWatermark()
        {
            var watermark = _dbContext.Watermarks.FirstOrDefault();
            if (watermark != null)
                return watermark.LastSequenceId;
            return 0;
        }

        public void UpdateWatermark(long lastSequenceId)
        {
            var watermark =  _dbContext.Watermarks.FirstOrDefault();
            if (watermark != null)
            {
                watermark.LastSequenceId = lastSequenceId;
            }
            watermark = new Watermark()
            {
                Id = Guid.NewGuid(),
                LastSequenceId = lastSequenceId
            };
            _dbContext.Watermarks.Add(watermark);
            _dbContext.SaveChanges();
        }
    }
}