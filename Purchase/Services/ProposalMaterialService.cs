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

    public async Task<List<ProposalMaterial>> GetAllAsync()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .Include(pm => pm.Proposal)
            .Include(pm => pm.Catalog)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ProposalMaterial>> GetByProposalIdAsync(int proposalId)
    {
        if (proposalId <= 0) return new List<ProposalMaterial>();

        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .Where(pm => pm.ProposalId == proposalId)
            .Include(pm => pm.Catalog)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProposalMaterial?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .Include(pm => pm.Proposal)
            .Include(pm => pm.Catalog)
            .FirstOrDefaultAsync(pm => pm.ID == id);
    }

    public async Task CreateAsync(ProposalMaterial item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

                if (string.IsNullOrWhiteSpace(item.NameMaterial))
            throw new ArgumentException("Название материала обязательно");

        await using var context = await CreateDbContextAsync();

        // Автозаполнение из каталога если указан CatalogId
        if (item.CatalogId.HasValue && item.CatalogId.Value > 0)
        {
            var catalog = await context.ProposalCatalogs
                .FirstOrDefaultAsync(pc => pc.ID == item.CatalogId.Value);

            if (catalog != null)
            {
                item.FillFromCatalog(catalog);

                // Если материал из каталога - автоматически ставим статус "Есть в базе"
                if (item.StatusM == MaterialStatus.New)
                {
                    item.StatusM = MaterialStatus.InCatalog;
                }
            }
        }

        await context.ProposalMaterials.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProposalMaterial updated)
    {
        if (updated == null) throw new ArgumentNullException(nameof(updated));

        await using var context = await CreateDbContextAsync();

        // Обновляем статус если материал связан с каталогом
        if (updated.CatalogId.HasValue && updated.StatusM == MaterialStatus.New)
        {
            updated.StatusM = MaterialStatus.InCatalog;
        }

        context.ProposalMaterials.Update(updated);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0) return;

        await using var context = await CreateDbContextAsync();
        var item = await context.ProposalMaterials.FindAsync(id);

        if (item != null)
        {
            context.ProposalMaterials.Remove(item);
            await context.SaveChangesAsync();
        }
    }

    public async Task<ProposalMaterial?> CreateFromCatalogAsync(int catalogId, int proposalId, int quantity, string comment) // ✅ int
    {
        if (catalogId <= 0 || proposalId <= 0 || quantity <= 0)
            return null;

        await using var context = await CreateDbContextAsync();
        var catalog = await context.ProposalCatalogs.FindAsync(catalogId);

        if (catalog == null)
            return null;

        var material = new ProposalMaterial
        {
            ProposalId = proposalId,
            CatalogId = catalogId,
            Quantity = quantity, // ✅ int
            Comment = comment,
            StatusM = MaterialStatus.InCatalog
        };

        material.FillFromCatalog(catalog);

        await context.ProposalMaterials.AddAsync(material);
        await context.SaveChangesAsync();

        return material;
    }

    public async Task<bool> ProposalExistsAsync(int proposalId)
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals.AnyAsync(p => p.ID == proposalId);
    }
}

public interface IProposalMaterialService
{
    Task<List<ProposalMaterial>> GetAllAsync();
    Task<List<ProposalMaterial>> GetByProposalIdAsync(int proposalId);
    Task<ProposalMaterial?> GetByIdAsync(int id);
    Task CreateAsync(ProposalMaterial item);
    Task UpdateAsync(ProposalMaterial item);
    Task DeleteAsync(int id);
    Task<ProposalMaterial?> CreateFromCatalogAsync(int catalogId, int proposalId, int quantity, string comment); // ✅ int
    Task<bool> ProposalExistsAsync(int proposalId);
}