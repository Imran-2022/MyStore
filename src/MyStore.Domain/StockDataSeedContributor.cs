using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyStore.Stocks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MyStore;

public class StockDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Stock, Guid> _stockRepository;

        public StockDataSeedContributor(IRepository<Stock, Guid> stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _stockRepository.CountAsync() > 0)
            {
                return; // Already seeded
            }

            var stockList = new List<Stock>
            {
                new Stock(Guid.NewGuid(), "Apple", "Warehouse A", 500),
                new Stock(Guid.NewGuid(), "Banana", "Warehouse A", 200),
                new Stock(Guid.NewGuid(), "Orange", "Warehouse B", 300),
                new Stock(Guid.NewGuid(), "Mango", "Warehouse B", 150),
                new Stock(Guid.NewGuid(), "Pineapple", "Warehouse C", 80)
            };

            foreach (var stock in stockList)
            {
                await _stockRepository.InsertAsync(stock, autoSave: true);
            }
        }
    }