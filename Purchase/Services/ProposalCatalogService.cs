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

    public async Task<List<ProposalCatalog>> GetAll()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCatalogs.ToListAsync();
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
        var existing = await context.ProposalCatalogs.FirstOrDefaultAsync(x => x.ID == updated.ID);

        if (existing == null)
            return;

        existing.Material = updated.Material;
        existing.Category = updated.Category;

        await context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        await using var context = await CreateDbContextAsync();
        var item = await context.ProposalCatalogs.FindAsync(id);

        if (item != null)
        {
            context.ProposalCatalogs.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}
public interface IProposalCatalogService
{
    Task<List<ProposalCatalog>> GetAll();
    Task Create(ProposalCatalog catalog);
    Task Update(ProposalCatalog catalog);
    Task Delete(int id);
}


