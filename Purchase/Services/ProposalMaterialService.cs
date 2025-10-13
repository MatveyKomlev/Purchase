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
        return await context.ProposalMaterials.ToListAsync();
    }

    public async Task<List<ProposalMaterial>> GetByProposalId(int proposalId)
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .Where(p => p.ProposalId == proposalId)
            .ToListAsync();
    }

    public async Task Create(ProposalMaterial item)
    {
        if (string.IsNullOrWhiteSpace(item.NameMaterial) || item.Quantity <= 0)
            return;

        await using var context = await CreateDbContextAsync();
        await context.ProposalMaterials.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task Update(ProposalMaterial updated)
    {
        await using var context = await CreateDbContextAsync();
        var existing = await context.ProposalMaterials.FirstOrDefaultAsync(x => x.ID == updated.ID);

        if (existing == null)
            return;

        existing.NameMaterial = updated.NameMaterial;
        existing.CategoryMaterial = updated.CategoryMaterial;
        existing.Code = updated.Code;
        existing.Quantity = updated.Quantity;
        existing.Comment = updated.Comment;

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
}
public interface IProposalMaterialService
{
    Task<List<ProposalMaterial>> GetAll();
    Task<List<ProposalMaterial>> GetByProposalId(int proposalId);
    Task Create(ProposalMaterial item);
    Task Update(ProposalMaterial item);
    Task Delete(int id);
}