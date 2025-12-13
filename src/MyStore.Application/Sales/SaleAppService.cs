using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyStore.Stocks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Sales
{
    public class SaleAppService : ApplicationService, ISaleAppService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IRepository<Stock, Guid> _stockRepository;

        public SaleAppService(ISaleRepository saleRepository, IRepository<Stock, Guid> stockRepository)
        {
            _saleRepository = saleRepository;
            _stockRepository = stockRepository;
        }

        public async Task<List<SaleDto>> GetListAsync()
        {
            var sales = await _saleRepository.GetListWithProductsAsync();
            // Using ObjectMapper to map entities -> DTOs
            return ObjectMapper.Map<List<Sale>, List<SaleDto>>(sales);
        }

        public async Task<SaleDto> GetAsync(Guid id)
        {
            var sale = await _saleRepository.GetWithProductsAsync(id)
                       ?? throw new UserFriendlyException("Sale not found");

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        // public async Task<SaleDto> CreateAsync(CreateUpdateSaleDto input)
        // {
        //     var sale = new Sale(
        //         GuidGenerator.Create(),
        //         input.Customer,
        //         input.DateTime,
        //         input.Products.Select(p =>
        //             new SaleProduct(
        //                 GuidGenerator.Create(),
        //                 p.Warehouse,
        //                 p.Product,
        //                 p.Quantity,
        //                 p.Price
        //             )
        //         ).ToList()
        //     );

        //     await _saleRepository.InsertAsync(sale, autoSave: true);

        //     return ObjectMapper.Map<Sale, SaleDto>(sale);
        // }

        public async Task<SaleDto> CreateAsync(CreateUpdateSaleDto input)
        {
            var sale = new Sale(
                GuidGenerator.Create(),
                input.Customer,
                input.DateTime,
                input.Products.Select(p =>
                    new SaleProduct(
                        GuidGenerator.Create(),
                        p.Warehouse,
                        p.Product,
                        p.Quantity,
                        p.Price
                    )
                ).ToList()
            );

            // 🔥 STOCK HANDLING (SALE REDUCTION)
            foreach (var product in sale.Products)
            {
                var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                    s.Product.ToLower() == product.Product.ToLower() &&
                    s.Warehouse.ToLower() == product.Warehouse.ToLower()
                );

                if (stock == null)
                {
                    throw new UserFriendlyException($"No stock for product {product.Product} in {product.Warehouse}");
                }

                if (stock.Quantity < product.Quantity)
                {
                    throw new UserFriendlyException($"Not enough stock for {product.Product} in {product.Warehouse}");
                }

                stock.ReduceOrClear(product.Quantity);

                if (stock.IsEmpty())
                {
                    await _stockRepository.DeleteAsync(stock);
                }
                else
                {
                    await _stockRepository.UpdateAsync(stock);
                }
            }

            await _saleRepository.InsertAsync(sale, autoSave: true);
            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }


        // public async Task DeleteAsync(Guid id)
        // {
        //     var sale = await _saleRepository.FindAsync(id);
        //     if (sale == null)
        //     {
        //         throw new UserFriendlyException("Sale not found");
        //     }

        //     await _saleRepository.DeleteAsync(sale, autoSave: true);
        // }
        public async Task DeleteAsync(Guid id)
        {
            var sale = await _saleRepository.GetWithProductsAsync(id)
                       ?? throw new UserFriendlyException("Sale not found");

            foreach (var product in sale.Products)
            {
                var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                    s.Product.ToLower() == product.Product.ToLower() &&
                    s.Warehouse.ToLower() == product.Warehouse.ToLower()
                );

                if (stock == null)
                {
                    // If stock entry doesn't exist, create it
                    await _stockRepository.InsertAsync(new Stock(
                        Guid.NewGuid(),
                        product.Product,
                        product.Warehouse,
                        product.Quantity
                    ));
                }
                else
                {
                    // Return sold quantity back to stock
                    stock.Increase(product.Quantity);

                    // ✅ Check if stock is empty and remove if necessary
                    if (stock.IsEmpty())
                    {
                        await _stockRepository.DeleteAsync(stock);
                    }
                    else
                    {
                        await _stockRepository.UpdateAsync(stock);
                    }
                }
            }

            // Delete sale after stock adjustment
            await _saleRepository.DeleteAsync(sale, autoSave: true);
        }

        // public async Task<SaleDto> UpdateAsync(Guid id, CreateUpdateSaleDto input)
        // {
        //     var sale = await _saleRepository.GetWithProductsAsync(id);
        //     if (sale == null)
        //     {
        //         throw new UserFriendlyException("Sale not found");
        //     }

        //     // Update root fields
        //     sale.Customer = input.Customer;
        //     sale.DateTime = input.DateTime;

        //     // 🔥 Replace child collection (BEST PRACTICE)
        //     sale.Products.Clear();

        //     foreach (var p in input.Products)
        //     {
        //         sale.Products.Add(
        //             new SaleProduct(
        //                 GuidGenerator.Create(),
        //                 p.Warehouse,
        //                 p.Product,
        //                 p.Quantity,
        //                 p.Price
        //             )
        //         );
        //     }

        //     await _saleRepository.UpdateAsync(sale, autoSave: true);

        //     return ObjectMapper.Map<Sale, SaleDto>(sale);
        // }

        public async Task<SaleDto> UpdateAsync(Guid id, CreateUpdateSaleDto input)
        {
            var sale = await _saleRepository.GetWithProductsAsync(id)
                       ?? throw new UserFriendlyException("Sale not found");

            // Create lookup of old products
            var oldProducts = sale.Products.ToDictionary(
                p => (p.Product.ToLower(), p.Warehouse.ToLower()),
                p => p
            );

            // Create lookup of new products
            var newProducts = input.Products.ToDictionary(
                p => (p.Product.ToLower(), p.Warehouse.ToLower()),
                p => p
            );

            // Adjust stock
            foreach (var oldItem in oldProducts)
            {
                if (!newProducts.TryGetValue(oldItem.Key, out var newItem))
                {
                    // Product removed from sale → increase stock back
                    await IncreaseStockAsync(oldItem.Value.Product, oldItem.Value.Warehouse, oldItem.Value.Quantity);
                }
                else
                {
                    var diff = oldItem.Value.Quantity - newItem.Quantity;

                    if (diff > 0)
                    {
                        // Sold less → return stock
                        await IncreaseStockAsync(oldItem.Value.Product, oldItem.Value.Warehouse, diff);
                    }
                    else if (diff < 0)
                    {
                        // Sold more → reduce stock
                        await ReduceStockAsync(oldItem.Value.Product, oldItem.Value.Warehouse, -diff);
                    }
                }
            }

            // Newly added products
            foreach (var newItem in newProducts)
            {
                if (!oldProducts.ContainsKey(newItem.Key))
                {
                    await ReduceStockAsync(newItem.Value.Product, newItem.Value.Warehouse, newItem.Value.Quantity);
                }
            }

            // Update sale entity
            sale.Customer = input.Customer;
            sale.DateTime = input.DateTime;
            sale.Products.Clear();
            foreach (var p in input.Products)
            {
                sale.Products.Add(new SaleProduct(
                    GuidGenerator.Create(),
                    p.Warehouse,
                    p.Product,
                    p.Quantity,
                    p.Price
                ));
            }

            await _saleRepository.UpdateAsync(sale, autoSave: true);
            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }
        private async Task IncreaseStockAsync(string product, string warehouse, int quantity)
        {
            var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                s.Product.ToLower() == product.ToLower() &&
                s.Warehouse.ToLower() == warehouse.ToLower()
            );

            if (stock == null)
            {
                await _stockRepository.InsertAsync(new Stock(Guid.NewGuid(), product, warehouse, quantity));
            }
            else
            {
                stock.Increase(quantity);
                await _stockRepository.UpdateAsync(stock);
            }
        }

        private async Task ReduceStockAsync(string product, string warehouse, int quantity)
        {
            var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                s.Product.ToLower() == product.ToLower() &&
                s.Warehouse.ToLower() == warehouse.ToLower()
            );

            if (stock == null)
                throw new UserFriendlyException($"Stock not found for {product} in {warehouse}");

            if (stock.Quantity < quantity)
                throw new UserFriendlyException($"Not enough stock for {product} in {warehouse}");

            stock.ReduceOrClear(quantity);

            if (stock.IsEmpty())
                await _stockRepository.DeleteAsync(stock);
            else
                await _stockRepository.UpdateAsync(stock);
        }

    }
}




// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Volo.Abp;
// using Volo.Abp.Application.Services;

// namespace MyStore.Sales
// {
//     public class SaleAppService : ApplicationService, ISaleAppService
//     {
//         private readonly ISaleRepository _saleRepository;

//         public SaleAppService(ISaleRepository saleRepository)
//         {
//             _saleRepository = saleRepository;
//         }

//         public async Task<List<SaleDto>> GetListAsync()
//         {
//             var sales = await _saleRepository.GetListWithProductsAsync();
//             return sales.Select(s => new SaleDto
//             {
//                 Id = s.Id,
//                 Customer = s.Customer,
//                 DateTime = s.DateTime,
//                 Products = s.Products.Select(p => new SaleProductDto
//                 {
//                     Warehouse = p.Warehouse,
//                     Product = p.Product,
//                     Quantity = p.Quantity,
//                     Price = p.Price
//                 }).ToList()
//             }).ToList();
//         }

//         public async Task<SaleDto> GetAsync(Guid id)
//         {
//             var sale = await _saleRepository.GetWithProductsAsync(id)
//                        ?? throw new UserFriendlyException("Sale not found");

//             return new SaleDto
//             {
//                 Id = sale.Id,
//                 Customer = sale.Customer,
//                 DateTime = sale.DateTime,
//                 Products = sale.Products.Select(p => new SaleProductDto
//                 {
//                     Warehouse = p.Warehouse,
//                     Product = p.Product,
//                     Quantity = p.Quantity,
//                     Price = p.Price
//                 }).ToList()
//             };
//         }

//         public async Task<SaleDto> CreateAsync(CreateUpdateSaleDto input)
//         {
//             var sale = new Sale(Guid.NewGuid(), input.Customer, input.DateTime,
//                 input.Products.Select(p => new SaleProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)).ToList()
//             );

//             await _saleRepository.InsertAsync(sale, autoSave: true);

//             return new SaleDto
//             {
//                 Id = sale.Id,
//                 Customer = sale.Customer,
//                 DateTime = sale.DateTime,
//                 Products = sale.Products.Select(p => new SaleProductDto
//                 {
//                     Warehouse = p.Warehouse,
//                     Product = p.Product,
//                     Quantity = p.Quantity,
//                     Price = p.Price
//                 }).ToList()
//             };
//         }
//     }
// }
