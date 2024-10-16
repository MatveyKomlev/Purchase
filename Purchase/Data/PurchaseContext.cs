using Microsoft.EntityFrameworkCore;

namespace Purchase.Data
{
    public class PurchaseContext : DbContext
    {
        public DbSet<Proposal> Proposals { get; set;}
    public PurchaseContext(DbContextOptions<PurchaseContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
