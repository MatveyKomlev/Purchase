using Microsoft.EntityFrameworkCore;
using Purchase.Data;

public class PurchaseContext : DbContext
{
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<ProposalMaterial> ProposalMaterials { get; set; }
    public DbSet<ProposalCatalog> ProposalCatalogs { get; set; }

    public PurchaseContext(DbContextOptions<PurchaseContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PROPOSAL CONFIGURATION
        modelBuilder.Entity<Proposal>(entity =>
        {
            // Конвертация enum в string
            entity.Property(p => p.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            entity.Property(p => p.Priority)
                  .HasConversion<string>()
                  .HasMaxLength(15);

            // Уникальный индекс для номера заявки
            entity.HasIndex(p => p.Number)
                  .IsUnique();

            // Ограничения длины для строковых полей
            entity.Property(p => p.Number).HasMaxLength(100);
            entity.Property(p => p.Author).HasMaxLength(30);
            entity.Property(p => p.Department).HasMaxLength(20);
            entity.Property(p => p.Explanation).HasMaxLength(500);

            entity.Ignore(p => p.PositionsCount);
        });

        // PROPOSAL MATERIAL
        modelBuilder.Entity<ProposalMaterial>(entity =>
        {
            // Связь с Proposal 
            entity.HasOne(pm => pm.Proposal)
                  .WithMany(p => p.Materials)
                  .HasForeignKey(pm => pm.ProposalId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с ProposalCatalog 
            entity.HasOne(pm => pm.Catalog)
                  .WithMany(pc => pc.ProposalMaterials)
                  .HasForeignKey(pm => pm.CatalogId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Конвертация enum для статуса материала
            entity.Property(pm => pm.StatusM)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            // Ограничения длины для строковых полей
            entity.Property(pm => pm.NameMaterial).HasMaxLength(200);
            entity.Property(pm => pm.CategoryMaterial).HasMaxLength(100);
            entity.Property(pm => pm.Code).HasMaxLength(50);
            entity.Property(pm => pm.Comment).HasMaxLength(500);
            entity.Property(pm => pm.ManufacturerPartNumber).HasMaxLength(100);
            entity.Property(pm => pm.ManufacturerName).HasMaxLength(150);
            entity.Property(pm => pm.UnitOfMeasure).HasMaxLength(20);

            entity.Ignore(pm => pm.TotalPrice);
        });

        // PROPOSAL CATALOG
        modelBuilder.Entity<ProposalCatalog>(entity =>
        {
            // Ограничения длины для строковых полей
            entity.Property(pc => pc.Material)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(pc => pc.Category)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(pc => pc.ManufacturerPartNumber).HasMaxLength(100);
            entity.Property(pc => pc.ManufacturerName).HasMaxLength(150);
            entity.Property(pc => pc.UnitOfMeasure).HasMaxLength(20);

            // Уникальный индекс для артикула производителя
            entity.HasIndex(pc => pc.ManufacturerPartNumber)
                  .IsUnique();
        });
    }

    // Переопределение SaveChanges для автоматических полей
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