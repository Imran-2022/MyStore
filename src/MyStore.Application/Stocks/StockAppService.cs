using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyStore.Stocks
{
    public class StockAppService : ApplicationService, IStockAppService
    {
        private readonly StockManager _stockManager;

        public StockAppService(StockManager stockManager)
        {
            _stockManager = stockManager;
        }

        public async Task<List<StockDto>> GetListAsync()
        {
            // Application service calls domain service instead of repository
            var stocks = await _stockManager.GetAllAsync();
            return ObjectMapper.Map<List<Stock>, List<StockDto>>(stocks);
        }

        public async Task IncreaseStockAsync(string product, string warehouse, int quantity)
        {
            await _stockManager.IncreaseAsync(product, warehouse, quantity);
        }

        public async Task ReduceStockAsync(string product, string warehouse, int quantity)
        {
            await _stockManager.ReduceAsync(product, warehouse, quantity);
        }
    }
}
