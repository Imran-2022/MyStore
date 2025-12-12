using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Purchases
{
    public interface IPurchaseRepository : IRepository<Purchase, Guid>
    {
        Task<Purchase> GetByCodeAsync(string purchaseCode);
         // Eager load helpers
        Task<Purchase> GetByIdWithProductsAsync(Guid id);
        Task<List<Purchase>> GetListWithProductsAsync();
    }
    
}
