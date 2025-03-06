using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Purchase.Data;


namespace Purchase.Services
{
    public class PurchaseService : IMyService
    {
        private readonly IDbContextFactory<PurchaseContext> purchaseFactory; // Поле доступное всем функциям

        // Конструктор, инициализирующий необходимые поля (порядок не имеет значения)
        public PurchaseService(IDbContextFactory<PurchaseContext> purchaseFactory)
        {
            this.purchaseFactory = purchaseFactory;
        }

        // Пример функции с получением списка элементов из базы.
        

        // Create
        public async Task Create(Proposal newProposal)
        {
            using var context = purchaseFactory.CreateDbContext();
            await context.Proposals.AddAsync(newProposal);
            await context.SaveChangesAsync();
        }
        // Read
        public async Task<List<Proposal>> GetAllProposalsAsync()
        {
            using var context = purchaseFactory.CreateDbContext();
            return await context.Proposals.ToListAsync();
        }

        public async Task<Proposal> GetProposalByIdAsync(int id)
        {
            using var context = purchaseFactory.CreateDbContext();
            return await context.Proposals.FindAsync(id);
        }

        // Update
        public async Task Update(Proposal updatedProposal)
        {
            using var context = purchaseFactory.CreateDbContext();
            context.Proposals.Update(updatedProposal);
            await context.SaveChangesAsync();
        }

        // Delete
        public async Task Delete(int id)
        {
            using var context = purchaseFactory.CreateDbContext();
            var proposal = await context.Proposals.FindAsync(id);
            if (proposal != null)
            {
                context.Proposals.Remove(proposal);
                await context.SaveChangesAsync();
            }
        }
    }


    public interface IMyService
    {
        Task Create(Proposal newProposal);
        Task<List<Proposal>> GetAllProposalsAsync();
        Task<Proposal> GetProposalByIdAsync(int id);
        Task Update(Proposal updatedProposal);
        Task Delete(int id);
    }


}

//public List<Proposal> GetAllItems()
//{
//    // Используем фабрику для создания контекста
//    using var context = purchaseFactory.CreateDbContext();
//    // Достаём из DbSet-свойства контекста все записи
//    return context.Proposals.ToList();
//}