using Microsoft.EntityFrameworkCore;
using Purchase.Data;

public class PurchaseContext : DbContext
{
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<ProposalCategory> ProposalCategories { get; set; }
    public PurchaseContext(DbContextOptions<PurchaseContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns(); 
        base.OnModelCreating(modelBuilder); 
    }
}
