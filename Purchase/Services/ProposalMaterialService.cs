using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services;

public class ProposalMaterialService : IProposalMaterialService
{
    private readonly IDbContextFactory<PurchaseContext> _contextFactory;

    public ProposalMaterialService(IDbContextFactory<PurchaseContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private async Task<PurchaseContext> CreateDbContextAsync()
    {
        return await _contextFactory.CreateDbContextAsync();
    }

    public async Task<List<ProposalMaterial>> GetAll()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .Include(pm => pm.Proposal)
            .Include(pm => pm.Catalog)
            .ToListAsync();
    }

    public async Task<List<ProposalMaterial>> GetByProposalId(int proposalId)
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .Where(p => p.ProposalId == proposalId)
            .Include(pm => pm.Catalog)
            .ToListAsync();
    }

    public async Task<ProposalMaterial?> GetById(int id)
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .AsTracking()
            .Include(pm => pm.Proposal)
            .Include(pm => pm.Catalog)
            .FirstOrDefaultAsync(pm => pm.ID == id);
    }

    public async Task Create(ProposalMaterial item)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item.NameMaterial) || item.Quantity <= 0)
                return;

            await using var context = await CreateDbContextAsync();

            // Если указан CatalogId - подтягиваем данные из каталога
            if (item.CatalogId.HasValue)
            {
                var catalog = await context.ProposalCatalogs
                    .FirstOrDefaultAsync(pc => pc.ID == item.CatalogId.Value);

                if (catalog != null)
                {
                    // Автозаполнение полей из каталога
                    item.ManufacturerPartNumber ??= catalog.ManufacturerPartNumber;
                    item.ManufacturerName ??= catalog.ManufacturerName;
                    item.UnitOfMeasure ??= catalog.UnitOfMeasure;
                }
            }

            await context.ProposalMaterials.AddAsync(item);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка при создании материала: {ex.Message}");
            throw; // Перебрасываем исключение дальше
        }
    }

    public async Task Update(ProposalMaterial updated)
    {
        await using var context = await CreateDbContextAsync();
        var existing = await context.ProposalMaterials.AsTracking()
            .FirstOrDefaultAsync(x => x.ID == updated.ID);

        if (existing == null)
            return;

        // Обновляем все поля
        existing.NameMaterial = updated.NameMaterial;
        existing.CategoryMaterial = updated.CategoryMaterial;
        existing.Code = updated.Code;
        existing.Quantity = updated.Quantity;
        existing.Comment = updated.Comment;
        existing.StatusM = updated.StatusM;
        existing.ManufacturerPartNumber = updated.ManufacturerPartNumber;
        existing.ManufacturerName = updated.ManufacturerName;
        existing.UnitOfMeasure = updated.UnitOfMeasure;
        existing.EstimatedPrice = updated.EstimatedPrice;
        existing.CatalogId = updated.CatalogId;

        await context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        await using var context = await CreateDbContextAsync();
        var item = await context.ProposalMaterials.FindAsync(id);

        if (item != null)
        {
            context.ProposalMaterials.Remove(item);
            await context.SaveChangesAsync();
        }
    }

    // Новый метод для автозаполнения из каталога
    public async Task<ProposalMaterial?> CreateFromCatalog(int catalogId, int proposalId, int quantity, string comment)
    {
        await using var context = await CreateDbContextAsync();
        var catalog = await context.ProposalCatalogs.FindAsync(catalogId);

        if (catalog == null)
            return null;

        var material = new ProposalMaterial
        {
            ProposalId = proposalId,
            CatalogId = catalogId,
            NameMaterial = catalog.Material,
            CategoryMaterial = catalog.Category,
            ManufacturerPartNumber = catalog.ManufacturerPartNumber,
            ManufacturerName = catalog.ManufacturerName,
            UnitOfMeasure = catalog.UnitOfMeasure,
            Quantity = quantity,
            Comment = comment,
            StatusM = "Новый",
            Code = "AUTO" // или генерировать код
        };

        await context.ProposalMaterials.AddAsync(material);
        await context.SaveChangesAsync();

        return material;
    }
}

public interface IProposalMaterialService
{
    Task<List<ProposalMaterial>> GetAll();
    Task<List<ProposalMaterial>> GetByProposalId(int proposalId);
    Task<ProposalMaterial?> GetById(int id);
    Task Create(ProposalMaterial item);
    Task Update(ProposalMaterial item);
    Task Delete(int id);
    Task<ProposalMaterial?> CreateFromCatalog(int catalogId, int proposalId, int quantity, string comment);
}