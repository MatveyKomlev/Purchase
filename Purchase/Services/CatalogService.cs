using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services;

public class CatalogService : IProposalCatalogService
{
    private readonly IDbContextFactory<PurchaseContext> _contextFactory;

    public CatalogService(IDbContextFactory<PurchaseContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private async Task<PurchaseContext> CreateDbContextAsync()
    {
        return await _contextFactory.CreateDbContextAsync();
    }

    public async Task<List<ProposalCatalog>> GetAll()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .Include(pc => pc.ProposalMaterials)
            .ToListAsync();
    }

    public async Task<ProposalCatalog?> GetById(int id)
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .AsTracking()
            .Include(pc => pc.ProposalMaterials)
            .FirstOrDefaultAsync(pc => pc.ID == id);
    }

    public async Task Create(ProposalCatalog catalog)
    {
        if (string.IsNullOrWhiteSpace(catalog.Material) || string.IsNullOrWhiteSpace(catalog.Category))
            return;

        await using var context = await CreateDbContextAsync();
        await context.ProposalCatalogs.AddAsync(catalog);
        await context.SaveChangesAsync();
    }

    public async Task Update(ProposalCatalog updated)
    {
        await using var context = await CreateDbContextAsync();

        var existing = await context.ProposalCatalogs
            .AsTracking()
            .FirstOrDefaultAsync(x => x.ID == updated.ID);

        if (existing == null)
            return;

        await context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        await using var context = await CreateDbContextAsync();
        var item = await context.ProposalCatalogs
            .Include(pc => pc.ProposalMaterials)
            .FirstOrDefaultAsync(pc => pc.ID == id);

        if (item != null)
        {
            // Проверяем, нет ли ссылающихся материалов
            if (item.ProposalMaterials.Any())
            {
                throw new InvalidOperationException("Нельзя удалить материал из каталога, так как на него ссылаются в заявках");
            }

            context.ProposalCatalogs.Remove(item);
            await context.SaveChangesAsync();
        }
    }

    // Поиск по названию или артикулу
    public async Task<List<ProposalCatalog>> Search(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAll();

        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .Where(pc => pc.Material.Contains(searchTerm) ||
                        pc.ManufacturerPartNumber.Contains(searchTerm) ||
                        pc.Category.Contains(searchTerm))
            .ToListAsync();
    }
}

public interface IProposalCatalogService
{
    Task<List<ProposalCatalog>> GetAll();
    Task<ProposalCatalog?> GetById(int id);
    Task Create(ProposalCatalog catalog);
    Task Update(ProposalCatalog catalog);
    Task Delete(int id);
    Task<List<ProposalCatalog>> Search(string searchTerm);
}