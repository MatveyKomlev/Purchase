using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services;

public class ProposalCategoryService : IProposalCategoryService
{
    private readonly IDbContextFactory<PurchaseContext> _contextFactory;

    public ProposalCategoryService(IDbContextFactory<PurchaseContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private async Task<PurchaseContext> CreateDbContextAsync()
    {
        return await _contextFactory.CreateDbContextAsync();
    }

    public async Task<List<ProposalCategory>> GetAllCategories()
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalCategories.ToListAsync();
    }

    public async Task CreateCategory(ProposalCategory category)
    {
        if (string.IsNullOrWhiteSpace(category.Title) || string.IsNullOrWhiteSpace(category.Material))
            return;

        category.Status = "Сохранено";
        await using var context = await CreateDbContextAsync();
        await context.ProposalCategories.AddAsync(category);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCategory(ProposalCategory category)
    {
        await using var context = await CreateDbContextAsync();
        var existing = await context.ProposalCategories.FindAsync(category.ID);
        if (existing != null)
        {
            existing.Title = category.Title;
            existing.Material = category.Material;
            existing.ProposalId = category.ProposalId;
            existing.Status = "Сохранено";
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteCategory(int id)
    {
        await using var context = await CreateDbContextAsync();
        var cat = await context.ProposalCategories.FindAsync(id);
        if (cat != null)
        {
            context.ProposalCategories.Remove(cat);
            await context.SaveChangesAsync();
        }
    }
}

public interface IProposalCategoryService
{
    Task<List<ProposalCategory>> GetAllCategories();
    Task CreateCategory(ProposalCategory category);
    Task UpdateCategory(ProposalCategory category);
    Task DeleteCategory(int id);
}
