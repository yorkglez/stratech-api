using Microsoft.EntityFrameworkCore;
using StratechAPI.Model;

namespace StratechAPI
{
    public class RdsDbContext: DbContext
    {
        public RdsDbContext(DbContextOptions<RdsDbContext> options) : base(options) { }
        public DbSet<AudienceRecord> AudienceRecord { get; set; }

    }
}
