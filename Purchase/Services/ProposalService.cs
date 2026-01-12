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

    public async Task CreateAsync(Proposal newProposal, int? userId = null)
{
    // Валидация
    if (!await ValidateProposalAsync(newProposal))
        throw new ArgumentException("Некорректные данные заявки");

    await using var context = await CreateDbContextAsync();

    if (userId.HasValue)
    {
        var user = await context.Users.FindAsync(userId.Value);
        if (user != null)
        {
            newProposal.UserId = user.ID;
            newProposal.Author = user.FullName;     
            newProposal.Department = user.Department; 
        }
    }

    if (newProposal.DateCreation == default)
        newProposal.DateCreation = DateTime.Now;

    if (string.IsNullOrWhiteSpace(newProposal.Number))
        newProposal.Number = await GenerateProposalNumberAsync();

    await context.Proposals.AddAsync(newProposal);
    await context.SaveChangesAsync();
}

    public async Task<List<Proposal>> GetAllProposalsAsync()
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Include(p => p.Materials)
                .ThenInclude(m => m.Catalog) 
            .OrderByDescending(p => p.DateCreation)
            .AsNoTracking() 
            .ToListAsync();
    }

    public async Task<Proposal?> GetByIdAsync(int id)
    {
        if (id < 1) return null;

        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Include(p => p.Materials)
                .ThenInclude(m => m.Catalog) 
            .FirstOrDefaultAsync(p => p.ID == id);
    }

    public async Task UpdateAsync(Proposal updatedProposal)
    {
        if (!await ValidateProposalAsync(updatedProposal))
            throw new ArgumentException("Некорректные данные заявки");

        await using var context = await CreateDbContextAsync();

        context.Proposals.Update(updatedProposal);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
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

    public async Task ChangeStatusAsync(int proposalId, ErpStatus newStatus)
    {
        await using var context = await CreateDbContextAsync();
        var proposal = await context.Proposals
            .FirstOrDefaultAsync(p => p.ID == proposalId);

        if (proposal != null)
        {
            proposal.Status = newStatus;
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Proposal>> GetByStatusAsync(ErpStatus status)
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Where(p => p.Status == status)
            .Include(p => p.Materials)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetPositionsCountAsync(int proposalId)
    {
        await using var context = await CreateDbContextAsync();
        return await context.ProposalMaterials
            .CountAsync(pm => pm.ProposalId == proposalId);
    }

    public async Task<List<Proposal>> GetUserProposalsAsync(int userId, bool isAdmin = false)
    {
        await using var context = await CreateDbContextAsync();

        var query = context.Proposals
            .Include(p => p.Materials)
            .ThenInclude(m => m.Catalog)
            .AsQueryable();

        // Если не админ - показываем только свои заявки
        if (!isAdmin)
        {
            query = query.Where(p => p.UserId == userId);
        }

        return await query
            .OrderByDescending(p => p.DateCreation)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Proposal>> GetProposalsForCurrentUserAsync(int? userId, bool isAdmin)
    {
        if (userId == null) return new List<Proposal>();

        return await GetUserProposalsAsync(userId.Value, isAdmin);
    }

        public async Task<bool> ValidateProposalAsync(Proposal proposal)
    {
        if (proposal == null) return false;

        // Проверка обязательных полей
        if (string.IsNullOrWhiteSpace(proposal.Number) ||
            string.IsNullOrWhiteSpace(proposal.Author) ||
            string.IsNullOrWhiteSpace(proposal.Department))
            return false;

        if (proposal.ID == 0)
        {
            await using var context = await CreateDbContextAsync();
            var exists = await context.Proposals
                .AnyAsync(p => p.Number == proposal.Number);
            if (exists) return false;
        }

        return true;
    }

    public async Task<string> GenerateProposalNumberAsync()
    {
        await using var context = await CreateDbContextAsync();

        var year = DateTime.Now.Year;
        var lastNumber = await context.Proposals
            .Where(p => p.Number.StartsWith($"З-{year}-"))
            .OrderByDescending(p => p.Number)
            .Select(p => p.Number)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(lastNumber))
        {
            return $"З-{year}-0001";
        }

        // Извлекаем номер и увеличиваем
        var parts = lastNumber.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
        {
            return $"З-{year}-{(lastNum + 1):D4}";
        }

        return $"З-{year}-0001";
    }

    public async Task<bool> ExistsAsync(int id)
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .AnyAsync(p => p.ID == id);
    }

    public async Task<List<Proposal>> GetOverdueProposalsAsync()
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals
            .Where(p => p.Deadline.HasValue &&
                       p.Deadline.Value < DateTime.Now &&
                       p.Status != ErpStatus.Approved)
            .Include(p => p.Materials)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetTotalProposalsCountAsync()
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals.CountAsync();
    }
}

public interface IProposalService
{
    Task CreateAsync(Proposal newProposal, int? userId);
    Task<List<Proposal>> GetAllProposalsAsync();
    Task<Proposal?> GetByIdAsync(int id);
    Task UpdateAsync(Proposal updatedProposal);
    Task DeleteAsync(int id);

    Task ChangeStatusAsync(int proposalId, ErpStatus newStatus);
    Task<List<Proposal>> GetByStatusAsync(ErpStatus status);
    Task<int> GetPositionsCountAsync(int proposalId);

    Task<bool> ValidateProposalAsync(Proposal proposal);
    Task<string> GenerateProposalNumberAsync();
}