using Microsoft.EntityFrameworkCore;

namespace Purchase.Data
{
    public class PurchaseContext : DbContext
    {
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalMaterial> ProposalMaterials { get; set; }
        public DbSet<ProposalCatalog> ProposalCatalogs { get; set; }
        public DbSet<User> Users { get; set; }

        public PurchaseContext(DbContextOptions<PurchaseContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Proposal>(entity =>
            {
                entity.Property(p => p.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.Property(p => p.Priority)
                      .HasConversion<string>()
                      .HasMaxLength(15);

                entity.HasIndex(p => p.Number)
                      .IsUnique();

                entity.Property(p => p.Number).HasMaxLength(100);
                entity.Property(p => p.Author).HasMaxLength(100);
                entity.Property(p => p.Department).HasMaxLength(100);
                entity.Property(p => p.Explanation).HasMaxLength(1000);

                entity.Ignore(p => p.PositionsCount);

                entity.HasOne(p => p.User)
             .WithMany()
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ProposalMaterial>(entity =>
            {
                entity.HasOne(pm => pm.Proposal)
                      .WithMany(p => p.Materials)
                      .HasForeignKey(pm => pm.ProposalId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pm => pm.Catalog)
                      .WithMany(pc => pc.ProposalMaterials)
                      .HasForeignKey(pm => pm.CatalogId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(pm => pm.StatusM)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.Property(pm => pm.NameMaterial).HasMaxLength(200);
                entity.Property(pm => pm.CategoryMaterial).HasMaxLength(100);
                entity.Property(pm => pm.Code).HasMaxLength(50);
                entity.Property(pm => pm.Comment).HasMaxLength(1000);
                entity.Property(pm => pm.ManufacturerPartNumber).HasMaxLength(100);
                entity.Property(pm => pm.ManufacturerName).HasMaxLength(150);
                entity.Property(pm => pm.UnitOfMeasure).HasMaxLength(20);

                entity.Ignore(pm => pm.TotalPrice);
            });

            modelBuilder.Entity<ProposalCatalog>(entity =>
            {
                entity.Property(pc => pc.Material)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(pc => pc.Category)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(pc => pc.ManufacturerPartNumber).HasMaxLength(100);
                entity.Property(pc => pc.ManufacturerName).HasMaxLength(150);
                entity.Property(pc => pc.UnitOfMeasure).HasMaxLength(20);

                entity.HasIndex(pc => pc.ManufacturerPartNumber)
                      .IsUnique()
                      .HasFilter("\"ManufacturerPartNumber\" IS NOT NULL");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.FullName)
                      .HasMaxLength(100);

                entity.Property(u => u.Role)
                      .HasConversion<string>()
                      .HasMaxLength(10);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.HasIndex(u => u.Username)
                      .IsUnique();

                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });
        }

        public override int SaveChanges()
        {
            SetTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetTimestamps()
        {
            var entries = ChangeTracker.Entries<Proposal>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (entry.Entity.DateCreation == default)
                {
     
                    entry.Entity.DateCreation = DateTime.Now;
                }
            }
        }
    }
}