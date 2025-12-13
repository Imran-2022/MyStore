using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyStore.Sales;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MyStore.EntityFrameworkCore
{
    public class SaleDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ISaleRepository _saleRepository;

        public SaleDataSeedContributor(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Skip if data already exists
            if (await _saleRepository.CountAsync() > 0)
            {
                return;
            }

            var sales = new List<Sale>
            {
                new Sale(
                    Guid.NewGuid(),
                    customer: "John Doe",
                    dateTime: DateTime.Parse("2025-12-12T14:30"),
                    products: new List<SaleProduct>
                    {
                        new SaleProduct(Guid.NewGuid(), "Warehouse A", "Apple", 10, 2),
                        new SaleProduct(Guid.NewGuid(), "Warehouse B", "Orange", 5, 3)
                    }
                ),
                new Sale(
                    Guid.NewGuid(),
                    customer: "Jane Smith",
                    dateTime: DateTime.Parse("2025-12-12T15:00"),
                    products: new List<SaleProduct>
                    {
                        new SaleProduct(Guid.NewGuid(), "Warehouse A", "Banana", 8, 1.5m),
                        new SaleProduct(Guid.NewGuid(), "Warehouse C", "Pineapple", 2, 5)
                    }
                )
            };

            foreach (var sale in sales)
            {
                await _saleRepository.InsertAsync(sale, autoSave: true);
            }
        }
    }
}
