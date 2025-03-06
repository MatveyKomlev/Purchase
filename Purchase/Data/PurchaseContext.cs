using Microsoft.EntityFrameworkCore;
namespace Purchase.Data
{
    public class PurchaseContext : DbContext
    {
        public DbSet<Proposal> Proposals { get; set;}
        //public IDbContextFactory<PurchaseContext> PurchaseFactory { get; }

        public PurchaseContext(DbContextOptions<PurchaseContext> options) : base(options)
        { }

        //public PurchaseContext(IDbContextFactory<PurchaseContext> purchaseFactory)
        //{
        //    PurchaseFactory = purchaseFactory;
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

    }
}
