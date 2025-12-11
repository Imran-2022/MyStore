using System;
using System.Threading.Tasks;
using MyStore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; // <-- important

namespace MyStore.Purchases
{
    public class EfCorePurchaseRepository : EfCoreRepository<MyStoreDbContext, Purchase, Guid>, IPurchaseRepository
    {
        public EfCorePurchaseRepository(IDbContextProvider<MyStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<Purchase> GetByCodeAsync(string purchaseCode)
        {
            return await DbSet.FirstOrDefaultAsync(p => p.PurchaseCode == purchaseCode);
        }
    }
}
