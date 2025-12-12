using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyStore.Stocks;

public interface IStockAppService : IApplicationService
{
    Task<List<StockDto>> GetListAsync();
}
