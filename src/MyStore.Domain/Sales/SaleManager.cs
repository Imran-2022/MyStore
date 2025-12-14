using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp;
using MyStore.Stocks;

namespace MyStore.Sales
{
    public class SaleManager : DomainService
    {
        private readonly StockManager _stockManager;

        public SaleManager(StockManager stockManager)
        {
            _stockManager = stockManager;
        }

        public async Task<Sale> CreateSaleAsync(
            string customer,
            DateTime dateTime,
            List<SaleProduct> products)
        {
            if (products == null || products.Count == 0)
                throw new BusinessException("Sale must contain at least one product");

            foreach (var p in products)
            {
                await _stockManager.ReduceAsync(p.Product, p.Warehouse, p.Quantity);
            }

            return new Sale(Guid.NewGuid(), customer, dateTime, products);
        }

        public async Task UpdateSaleAsync(
    Sale sale,
    List<SaleProduct> newProducts,
    string customer,
    DateTime dateTime)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale));

            if (newProducts == null || newProducts.Count == 0)
                throw new BusinessException("Sale must contain at least one product");

            // 1️⃣ Restore stock from OLD products
            foreach (var old in sale.Products)
            {
                await _stockManager.IncreaseAsync(
                    old.Product,
                    old.Warehouse,
                    old.Quantity
                );
            }

            // 2️⃣ Reduce stock for NEW products
            foreach (var p in newProducts)
            {
                await _stockManager.ReduceAsync(
                    p.Product,
                    p.Warehouse,
                    p.Quantity
                );
            }

            // 3️⃣ Update sale root fields
            sale.SetCustomer(customer);
            sale.SetDateTime(dateTime);

            // 4️⃣ FULL REPLACE products (delete + recreate)
            sale.ReplaceProducts(newProducts);
        }


        public async Task DeleteSaleAsync(Sale sale)
        {
            foreach (var p in sale.Products)
            {
                await _stockManager.IncreaseAsync(p.Product, p.Warehouse, p.Quantity);
            }
        }
    }
}
