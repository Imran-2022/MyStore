using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp;
using System.Collections.Generic;

namespace MyStore.Stocks
{
    public class StockManager : DomainService
    {
        private readonly IRepository<Stock, Guid> _stockRepository;

        public StockManager(IRepository<Stock, Guid> stockRepository)
        {
            _stockRepository = stockRepository;
        }
        public async Task<List<Stock>> GetAllAsync()
        {
            return await _stockRepository.GetListAsync();
        }


        public async Task IncreaseAsync(string product, string warehouse, int quantity)
        {
            if (quantity <= 0)
                throw new BusinessException("Stock:QuantityMustBeGreaterThanZero");

            var stock = await FindStockAsync(product, warehouse);

            if (stock == null)
            {
                stock = new Stock(Guid.NewGuid(), product, warehouse, quantity);
                await _stockRepository.InsertAsync(stock);
            }
            else
            {
                stock.Increase(quantity);
                await _stockRepository.UpdateAsync(stock);
            }
        }

        public async Task ReduceAsync(string product, string warehouse, int quantity)
        {
            if (quantity <= 0)
                throw new BusinessException("Stock:QuantityMustBeGreaterThanZero");

            var stock = await FindStockAsync(product, warehouse);

            if (stock == null)
                throw new BusinessException($"Stock not found for {product} in {warehouse}");

            // if (stock.Quantity < quantity)
            //     throw new BusinessException($"Not enough stock for {product} in {warehouse}");

            stock.ReduceOrClear(quantity);

            if (stock.IsEmpty())
                await _stockRepository.DeleteAsync(stock);
            else
                await _stockRepository.UpdateAsync(stock);
        }

        private async Task<Stock> FindStockAsync(string product, string warehouse)
        {
            return await _stockRepository.FirstOrDefaultAsync(s =>
                s.Product.ToLower() == product.ToLower() &&
                s.Warehouse.ToLower() == warehouse.ToLower()
            );
        }
    }
}
