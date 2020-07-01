using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PoCCommon.Database;
using PoCCommon.Database.Models;

namespace PoCCommon.Services
{
    public class WatermarkService
    {
        private readonly PocDbContext _dbContext;
        
        public WatermarkService(PocDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Watermark GetCurrentWatermark()
        {
            //This uses no tracking so we can reset the watermark of a running application
            var watermark = _dbContext.Watermarks.AsNoTracking().FirstOrDefault(x => x.Name == "PoCEventHandler");
            if (watermark is null)
            {
                watermark = new Watermark()
                {
                    Id = Guid.NewGuid()
                };
                _dbContext.Add(watermark);
                _dbContext.SaveChanges();
            }
            return watermark;
        }

        public bool UpdateWatermark(Watermark watermark)
        {
            
            _dbContext.Entry(_dbContext.Watermarks.FirstOrDefault(x => x.Name == "PoCEventHandler")).CurrentValues.SetValues(watermark);
            return _dbContext.SaveChanges() > 0;
        }
    }
}