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

        // Конфигурация для Proposal
        modelBuilder.Entity<Proposal>(entity =>
        {
            // Конвертация enum в string для Status
            entity.Property(p => p.Status)
                  .HasConversion<string>()
                  .HasMaxLength(10);

            // Конвертация enum в string для Priority
            entity.Property(p => p.Priority)
                  .HasConversion<string>()
                  .HasMaxLength(15);

            // Уникальный индекс для номера заявки
            entity.HasIndex(p => p.Number)
                  .IsUnique();

            // Ограничения длины для строковых полей
            entity.Property(p => p.Number)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(p => p.Author)
                  .HasMaxLength(30);

            entity.Property(p => p.Department)
                  .HasMaxLength(20);

            entity.Property(p => p.Explanation)
                  .HasMaxLength(500);
        });

        // Конфигурация для ProposalMaterial
        modelBuilder.Entity<ProposalMaterial>(entity =>
        {
            // Связь с Proposal (One-to-Many)
            entity.HasOne(pm => pm.Proposal)
                  .WithMany(p => p.Materials)
                  .HasForeignKey(pm => pm.ProposalId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с ProposalCatalog (Many-to-One) - ОПЦИОНАЛЬНАЯ
            entity.HasOne(pm => pm.Catalog)
                  .WithMany(pc => pc.ProposalMaterials)
                  .HasForeignKey(pm => pm.CatalogId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Ограничения длины для строковых полей
            entity.Property(pm => pm.NameMaterial)
                  .IsRequired();

            entity.Property(pm => pm.CategoryMaterial)
                  .IsRequired();

            entity.Property(pm => pm.Code)
                  .HasMaxLength(10)
                  .IsRequired();

            entity.Property(pm => pm.Comment)
                  .IsRequired();

            entity.Property(pm => pm.StatusM)
                  .IsRequired();

            entity.Property(pm => pm.ManufacturerPartNumber)
                  .HasMaxLength(50);

            entity.Property(pm => pm.ManufacturerName)
                  .HasMaxLength(100);

            entity.Property(pm => pm.UnitOfMeasure)
                  .HasMaxLength(20);

            // Настройка decimal полей
            entity.Property(pm => pm.EstimatedPrice)
                  .HasColumnType("decimal(18,2)");

            // Вычисляемое поле TotalPrice (не сохраняется в БД)
            entity.Ignore(pm => pm.TotalPrice);
        });

        // Конфигурация для ProposalCatalog
        modelBuilder.Entity<ProposalCatalog>(entity =>
        {
            // Ограничения длины для строковых полей
            entity.Property(pc => pc.Material)
                  .IsRequired();

            entity.Property(pc => pc.Category)
                  .IsRequired();

            entity.Property(pc => pc.ManufacturerPartNumber)
                  .HasMaxLength(50);

            entity.Property(pc => pc.ManufacturerName)
                  .HasMaxLength(100);

            entity.Property(pc => pc.UnitOfMeasure)
                  .HasMaxLength(20);
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