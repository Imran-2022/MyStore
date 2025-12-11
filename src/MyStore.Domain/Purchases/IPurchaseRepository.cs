using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Purchases
{
    public interface IPurchaseRepository : IRepository<Purchase, Guid>
    {
        Task<Purchase> GetByCodeAsync(string purchaseCode);
    }
}
