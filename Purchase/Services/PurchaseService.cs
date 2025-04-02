using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;
using Purchase.Data;

namespace Purchase.Services;

public class PurchaseService : IMyService
{
    private readonly IDbContextFactory<PurchaseContext> _purchaseFactory;

    public PurchaseService(IDbContextFactory<PurchaseContext> purchaseFactory)
    {
        _purchaseFactory = purchaseFactory;
    }

    private async Task<PurchaseContext> CreateDbContextAsync()
    {
        return await _purchaseFactory.CreateDbContextAsync();
    }

    // Create
    public async Task Create(Proposal newProposal)
    {
        if (string.IsNullOrWhiteSpace(newProposal.Author) || string.IsNullOrWhiteSpace(newProposal.Department)
            || string.IsNullOrWhiteSpace(newProposal.Category) || string.IsNullOrWhiteSpace(newProposal.Status))
            return;

        await using var context = await _purchaseFactory.CreateDbContextAsync();
        await context.Proposals.AddAsync(newProposal);
        await context.SaveChangesAsync();
    }

    // Read
    public async Task<List<Proposal>> GetAllProposals()
    {
        await using var context = await CreateDbContextAsync();
        return await context.Proposals.ToListAsync();
    }

    // Update
    public async Task Update(Proposal updatedProposal)
    {
        await using var context = await CreateDbContextAsync();
        var proposal = await context.Proposals.AsTracking().FirstOrDefaultAsync(x => x.ID == updatedProposal.ID);

        if (proposal == null)
            return;

        proposal.Author = updatedProposal.Author;
        proposal.Category = updatedProposal.Category;
        proposal.Department = updatedProposal.Department;
        proposal.Status = updatedProposal.Status;

        await context.SaveChangesAsync(); 
    }

    // Delete
    public async Task Delete(int id)
    {
        if (id < 1) return;

        await using var context = await CreateDbContextAsync();
        var proposal = await context.Proposals.FindAsync(id);
        
        if (proposal != null)
        {
            context.Proposals.Remove(proposal);
            await context.SaveChangesAsync();
            await ReorderNumbers();
        }
    }

    // Метод для перезаписи номеров в базе данных
    public async Task ReorderNumbers()
    {
        await using var context = await CreateDbContextAsync();
        var proposals = await context.Proposals.OrderBy(p => p.DateCreation).ToListAsync();

        int counter = 1;
        foreach (var proposal in proposals)
        {
            proposal.Number = counter++;
        }

        await context.SaveChangesAsync();
    }
}

public interface IMyService
{
    Task Create(Proposal newProposal);
    Task<List<Proposal>> GetAllProposals();
    Task Update(Proposal updatedProposal);
    Task Delete(int id);
}


