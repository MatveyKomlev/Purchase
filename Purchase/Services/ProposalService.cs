using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services;

public class ProposalService : IProposalService
{
    private readonly IDbContextFactory<PurchaseContext> _contextFactory;

    public ProposalService(IDbContextFactory<PurchaseContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private async Task<PurchaseContext> CreateDbContextAsync()
    {
        return await _contextFactory.CreateDbContextAsync();
    }

    // Create
    public async Task Create(Proposal newProposal)
    {
        if (string.IsNullOrWhiteSpace(newProposal.Author) ||
            string.IsNullOrWhiteSpace(newProposal.Department) ||
            string.IsNullOrWhiteSpace(newProposal.Number))
            return;

        await using var context = await CreateDbContextAsync();

        // Автоматическая установка даты создания если не указана
        if (newProposal.DateCreation == default)
            newProposal.DateCreation = DateTime.Now;

        await context.Proposals.AddAsync(newProposal);
        await context.SaveChangesAsync();
    }

    // Read
    public async Task<List<Proposal>> GetAllProposals()
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Include(p => p.Materials)
            .OrderByDescending(p => p.DateCreation)
            .ToListAsync();
    }

    public async Task<Proposal?> GetById(int id)
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Include(p => p.Materials)
            .FirstOrDefaultAsync(p => p.ID == id);
    }

    // Update
    public async Task Update(Proposal updatedProposal)
    {
        await using var context = await CreateDbContextAsync();
        var proposal = await context.Proposals.AsTracking()
            .FirstOrDefaultAsync(x => x.ID == updatedProposal.ID);

        if (proposal == null)
            return;

        // Обновляем все поля
        proposal.Number = updatedProposal.Number;
        proposal.Author = updatedProposal.Author;
        proposal.Department = updatedProposal.Department;
        proposal.Status = updatedProposal.Status;
        proposal.Deadline = updatedProposal.Deadline;
        proposal.Explanation = updatedProposal.Explanation;
        proposal.Priority = updatedProposal.Priority;

        await context.SaveChangesAsync();
    }

    // Delete
    public async Task Delete(int id)
    {
        if (id < 1) return;

        await using var context = await CreateDbContextAsync();
        var proposal = await context.Proposals
            .Include(p => p.Materials)
            .FirstOrDefaultAsync(p => p.ID == id);

        if (proposal != null)
        {
            context.Proposals.Remove(proposal);
            await context.SaveChangesAsync();
        }
    }

    // Бизнес-методы
    public async Task ChangeStatus(int proposalId, ErpStatus newStatus)
    {
        await using var context = await CreateDbContextAsync();
        var proposal = await context.Proposals.AsTracking()
            .FirstOrDefaultAsync(p => p.ID == proposalId);

        if (proposal != null)
        {
            proposal.Status = newStatus;
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Proposal>> GetByStatus(ErpStatus status)
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Where(p => p.Status == status)
            .Include(p => p.Materials)
            .ToListAsync();
    }

    public async Task<int> GetPositionsCount(int proposalId)
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .CountAsync(pm => pm.ProposalId == proposalId);
    }
}

public interface IProposalService
{
    Task Create(Proposal newProposal);
    Task<List<Proposal>> GetAllProposals();
    Task<Proposal?> GetById(int id);
    Task Update(Proposal updatedProposal);
    Task Delete(int id);
    Task ChangeStatus(int proposalId, ErpStatus newStatus);
    Task<List<Proposal>> GetByStatus(ErpStatus status);
    Task<int> GetPositionsCount(int proposalId);
}