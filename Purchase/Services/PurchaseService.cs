using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;
using Purchase.Data;

namespace Purchase.Services;
public class PurchaseService : IMyService
{
    private readonly DbContextOptions<PurchaseContext> _purchaseFactory;

    public PurchaseService(DbContextOptions<PurchaseContext> purchaseFactory)
    {
        _purchaseFactory = purchaseFactory;
    }
    private PurchaseContext CreateDbContext()
    {
        return new PurchaseContext(_purchaseFactory);
    }
    // Create
    public async Task Create(Proposal newProposal)
    {
        //using var context = _purchaseFactory.CreateDbContext();
        //await context.Proposals.AddAsync(newProposal);
        //await context.SaveChangesAsync();
        using (var context = CreateDbContext())
        {
            context.Proposals.Add(newProposal);
            await context.SaveChangesAsync();
        }
    }
    // Read
    public async Task<List<Proposal>> GetAllProposals()
    {
        using (var context = CreateDbContext())
        {
            return await context.Proposals.ToListAsync();
        }
    }

    //public async Task<Proposal> GetProposalByIdAsync(int id)
    //{
    //    using var context = _purchaseFactory.CreateDbContext();
    //    return await context.Proposals.FindAsync(id);
    //}

    // Update
    public async Task Update(Proposal updatedProposal)
    {
        using (var context = CreateDbContext())
        {
            context.Proposals.Update(updatedProposal);
            await context.SaveChangesAsync();
        }
    }

    // Delete
    public async Task Delete(int id)
    {
        using (var context = CreateDbContext())
        {
            var proposal = await context.Proposals.FindAsync(id);
            if (proposal != null)
            {
                context.Proposals.Remove(proposal);
                await context.SaveChangesAsync();
            }
        }
    }
}


public interface IMyService
{
    Task Create(Proposal newProposal);
    Task<List<Proposal>> GetAllProposals();
    //Task<Proposal> GetProposalByIdAsync(int id);
    Task Update(Proposal updatedProposal);
    Task Delete(int id);
}

