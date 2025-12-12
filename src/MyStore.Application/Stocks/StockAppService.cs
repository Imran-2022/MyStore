using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Stocks
{
    public class StockAppService : ApplicationService, IStockAppService
    {
        private readonly IRepository<Stock, Guid> _stockRepository;

        public StockAppService(IRepository<Stock, Guid> stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<List<StockDto>> GetListAsync()
        {
            var items = await _stockRepository.GetListAsync();

            return ObjectMapper.Map<List<Stock>, List<StockDto>>(items);
        }
    }
}
