using Microsoft.EntityFrameworkCore;
using PoCCommon.Database.Models;

namespace PoCCommon.Database
{
    public class PocDbContext : DbContext
    {
        public DbSet<Watermark> Watermarks { get; set; }
        
        public PocDbContext(DbContextOptions options) : base(options)
        {
        }
        
    }
}