using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using MyStore.EntityFrameworkCore;

namespace MyStore.Sales
{
    public class EfCoreSaleRepository : EfCoreRepository<MyStoreDbContext, Sale, Guid>, ISaleRepository
    {
        public EfCoreSaleRepository(IDbContextProvider<MyStoreDbContext> dbContextProvider) : base(dbContextProvider) { }

        public async Task<Sale> GetWithProductsAsync(Guid id)
        {
            return await (await GetDbSetAsync())
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Sale>> GetListWithProductsAsync()
        {
            return await (await GetDbSetAsync())
                .Include(x => x.Products)
                .ToListAsync();
        }
    }
}
