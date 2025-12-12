using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyStore.Purchases;

public interface IPurchaseAppService : IApplicationService
{
    Task<PurchaseDto> GetAsync(Guid id);
    Task<PurchaseDto> GetByCodeAsync(string purchaseCode);
    Task<List<PurchaseDto>> GetListAsync();
    Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input);
    Task UpdateAsync(Guid id, CreateUpdatePurchaseDto input);
    Task DeleteAsync(Guid id);
}
