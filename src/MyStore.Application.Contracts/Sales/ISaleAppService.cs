using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyStore.Sales
{
    public interface ISaleAppService : IApplicationService
    {
        Task<List<SaleDto>> GetListAsync();
        Task<SaleDto> GetAsync(Guid id);
        Task<SaleDto> CreateAsync(CreateUpdateSaleDto input);
    }
}
