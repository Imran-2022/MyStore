using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using MyStore.Sales;

namespace MyStore.Sales
{
    public interface ISaleRepository : IRepository<Sale, Guid>
    {
        Task<Sale> GetWithProductsAsync(Guid id);
        Task<List<Sale>> GetListWithProductsAsync();
    }
}
