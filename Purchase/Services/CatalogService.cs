using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services;

public class ProposalCatalogService : IProposalCatalogService 
{
    private readonly IDbContextFactory<PurchaseContext> _contextFactory;

    public ProposalCatalogService(IDbContextFactory<PurchaseContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private async Task<PurchaseContext> CreateDbContextAsync()
    {
        return await _contextFactory.CreateDbContextAsync();
    }

    public async Task<List<ProposalCatalog>> GetAllAsync()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .Include(pc => pc.ProposalMaterials)
            .AsNoTracking() 
            .ToListAsync();
    }

    public async Task<ProposalCatalog?> GetByIdAsync(int id) 
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .Include(pc => pc.ProposalMaterials)
            .FirstOrDefaultAsync(pc => pc.ID == id);
    }

    public async Task CreateAsync(ProposalCatalog catalog)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(catalog.Material) || string.IsNullOrWhiteSpace(catalog.Category))
                throw new ArgumentException("Название материала и категория обязательны");

            await using var context = await CreateDbContextAsync();

            // Проверка уникальности артикула (если указан)
            if (!string.IsNullOrEmpty(catalog.ManufacturerPartNumber))
            {
                var exists = await context.ProposalCatalogs
                    .AnyAsync(pc => pc.ManufacturerPartNumber == catalog.ManufacturerPartNumber && pc.ID != catalog.ID);
                if (exists)
                    throw new InvalidOperationException("Материал с таким артикулом уже существует");
            }

            await context.ProposalCatalogs.AddAsync(catalog);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка создания материала: {ex.Message}", ex);
        }
    }

    public async Task UpdateAsync(ProposalCatalog updated) 
    {
        if (updated == null) throw new ArgumentNullException(nameof(updated));

        await using var context = await CreateDbContextAsync();

        context.ProposalCatalogs.Update(updated);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await CreateDbContextAsync();
        var item = await context.ProposalCatalogs
            .Include(pc => pc.ProposalMaterials)
            .FirstOrDefaultAsync(pc => pc.ID == id);

        if (item == null) return;

        if (item.ProposalMaterials.Any())
        {
            throw new InvalidOperationException("Нельзя удалить материал из каталога, так как на него ссылаются в заявках");
        }

        context.ProposalCatalogs.Remove(item);
        await context.SaveChangesAsync();
    }

    public async Task<List<ProposalCatalog>> SearchAsync(string searchTerm) 
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .Where(pc => pc.Material.Contains(searchTerm) ||
                        (pc.ManufacturerPartNumber != null && pc.ManufacturerPartNumber.Contains(searchTerm)) ||
                        pc.Category.Contains(searchTerm))
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<List<string>> GetCategoriesAsync()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs
            .Select(pc => pc.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}

public interface IProposalCatalogService
{
    Task<List<ProposalCatalog>> GetAllAsync(); 
    Task<ProposalCatalog?> GetByIdAsync(int id); 
    Task CreateAsync(ProposalCatalog catalog);
    Task UpdateAsync(ProposalCatalog catalog);
    Task DeleteAsync(int id); 
    Task<List<ProposalCatalog>> SearchAsync(string searchTerm); 
    Task<List<string>> GetCategoriesAsync(); 
}