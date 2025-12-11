using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyStore.Purchases;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MyStore.EntityFrameworkCore
{
    public class PurchaseDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseDataSeedContributor(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Check if already exists
            if (await _purchaseRepository.CountAsync() > 0)
            {
                return; // Already seeded
            }

            // Sample purchases
            var purchases = new List<Purchase>
            {
                new Purchase(
                    purchaseCode: "20251211122128",
                    supplierName: "Imran",
                    dateTime: DateTime.Parse("2025-12-11T12:21"),
                    products: new List<PurchaseProduct>
                    {
                        new PurchaseProduct(Guid.NewGuid(), "Warehouse 1", "Apple", 100, 40),
                        new PurchaseProduct(Guid.NewGuid(), "Warehouse 2", "Pen", 50, 5)
                    },
                    discount: 500,
                    paidAmount: 2000
                ),

                new Purchase(
                    purchaseCode: "20251210113045",
                    supplierName: "Ali",
                    dateTime: DateTime.Parse("2025-12-10T11:30"),
                    products: new List<PurchaseProduct>
                    {
                        new PurchaseProduct(Guid.NewGuid(), "Warehouse 1", "Banana", 200, 2),
                        new PurchaseProduct(Guid.NewGuid(), "Warehouse 3", "Notebook", 30, 15)
                    },
                    discount: 100,
                    paidAmount: 400
                ),

                new Purchase(
                    purchaseCode: "20251209101530",
                    supplierName: "Sara",
                    dateTime: DateTime.Parse("2025-12-09T10:15"),
                    products: new List<PurchaseProduct>
                    {
                        new PurchaseProduct(Guid.NewGuid(), "Warehouse 2", "Orange", 150, 3),
                        new PurchaseProduct(Guid.NewGuid(), "Warehouse 3", "Pencil", 100, 1)
                    },
                    discount: 50,
                    paidAmount: 300
                )
            };

            foreach (var purchase in purchases)
            {
                await _purchaseRepository.InsertAsync(purchase);
            }
        }
    }
}
