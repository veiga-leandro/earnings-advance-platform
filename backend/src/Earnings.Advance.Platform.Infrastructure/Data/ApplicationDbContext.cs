using Earnings.Advance.Platform.Domain.Entities;
using Earnings.Advance.Platform.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Earnings.Advance.Platform.Infrastructure.Data
{
    /// <summary>
    /// Database context
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AdvanceRequest> AnticipationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies entities configuration
            modelBuilder.ApplyConfiguration(new AdvanceRequestConfiguration());
        }
    }
}
