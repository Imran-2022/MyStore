using System;
using System.Threading.Tasks;
using System.Collections.Generic; // <-- Required for List<Purchase>
using MyStore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; // <-- Important: Already there

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
            // 👇 ADD THE .Include() HERE
            return await DbSet
                .Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.PurchaseCode == purchaseCode);

            // 
            // return await DbSet.FirstOrDefaultAsync(p => p.PurchaseCode == purchaseCode);
            // this also work . but i tried for an issue. 
        }
        public async Task<Purchase> GetByIdWithProductsAsync(Guid id)
        {
            return await DbSet
                .Include(p => p.Products)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Purchase>> GetListWithProductsAsync()
        {
            return await DbSet
                .Include(p => p.Products)
                .ToListAsync();
        }

        public async Task<Purchase> GetWithProductsAsync(Guid id)
            {
                return await (await GetDbSetAsync())
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }


        // 💡 NEW CODE START: Override GetListAsync for eager loading
        // Optionally override GetListAsync if callers use repository.GetListAsync(includeDetails: true)
        public override async Task<List<Purchase>> GetListAsync(
            bool includeDetails = false,
            System.Threading.CancellationToken cancellationToken = default)
        {
            var query = await GetQueryableAsync();

            if (includeDetails)
            {
                // This is the crucial line: Eagerly load the nested collection
                // 'Products' must be the name of the List<PurchaseProduct> property on your Purchase entity.
                query = query.Include(p => p.Products);
            }

            return await query.ToListAsync(cancellationToken);
        }
        // 💡 NEW CODE END
    }
}