using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyStore.Stocks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Purchases
{
    public class PurchaseAppService : ApplicationService, IPurchaseAppService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly PurchaseManager _purchaseManager;
        private readonly IRepository<Stock, Guid> _stockRepository;


        public PurchaseAppService(
            IPurchaseRepository purchaseRepository,
            PurchaseManager purchaseManager,
            IRepository<Stock, Guid> stockRepository)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseManager = purchaseManager;
            _stockRepository = stockRepository;
        }

        public async Task<PurchaseDto> GetAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetAsync(id);
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase); // AutoMapper
        }

        public async Task<PurchaseDto> GetByCodeAsync(string purchaseCode)
        {
            var purchase = await _purchaseRepository.GetByCodeAsync(purchaseCode);
            if (purchase == null)
            {
                throw new UserFriendlyException($"Purchase with code '{purchaseCode}' not found.");
            }
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase); // AutoMapper
        }

        public async Task<List<PurchaseDto>> GetListAsync()
        {
            var purchases = await _purchaseRepository.GetListAsync(includeDetails: true);
            return ObjectMapper.Map<List<Purchase>, List<PurchaseDto>>(purchases); // AutoMapper
        }
        public async Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input)
        {
            if (input.Products == null || input.Products.Count == 0)
                throw new UserFriendlyException("Purchase must contain at least one product.");

            // Map DTO products -> domain products
            var domainProducts = input.Products.ConvertAll(p =>
                new PurchaseProduct(Guid.NewGuid(), p.Warehouse, p.Product, p.Quantity, p.Price)
            );

            // Create Purchase aggregate
            var purchase = _purchaseManager.CreatePurchase(
                purchaseCode: input.PurchaseCode,
                supplierName: input.SupplierName,
                dateTime: input.DateTime,
                products: domainProducts,
                discount: input.Discount,
                paidAmount: input.PaidAmount
            );

            // Save to DB
            await _purchaseRepository.InsertAsync(purchase, autoSave: true);

            // 🔥 STOCK HANDLING (CREATE ONLY)
            foreach (var product in purchase.Products)
            {
                var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                    s.Product.ToLower() == product.Product.ToLower() &&
                    s.Warehouse.ToLower() == product.Warehouse.ToLower()
                );

                if (stock != null)
                {
                    // Increase existing stock
                    stock.Increase(product.Quantity);
                    await _stockRepository.UpdateAsync(stock);
                }
                else
                {
                    // Create new stock entry
                    var newStock = new Stock(
                        Guid.NewGuid(),
                        product.Product,
                        product.Warehouse,
                        product.Quantity
                    );

                    await _stockRepository.InsertAsync(newStock);
                }
            }

            // Return DTO
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }
        // public async Task UpdateAsync(Guid id, CreateUpdatePurchaseDto input)
        // {
        //     var purchase = await _purchaseRepository.GetByIdWithProductsAsync(id)
        //         ?? throw new UserFriendlyException($"Purchase with id '{id}' not found.");

        //     if (input.Products == null || input.Products.Count == 0)
        //         throw new UserFriendlyException("Purchase must contain at least one product.");

        //     // Update simple props
        //     if (!string.Equals(purchase.SupplierName, input.SupplierName, StringComparison.Ordinal))
        //     {
        //         purchase.SetSupplierName(input.SupplierName);
        //     }

        //     purchase.SetDateTime(input.DateTime);
        //     purchase.SetDiscount(input.Discount);
        //     purchase.SetPaidAmount(input.PaidAmount);

        //     // Replace products (simple approach: create new product entities)
        //     var domainProducts = input.Products.Select(p => new PurchaseProduct(
        //         Guid.NewGuid(),
        //         p.Warehouse,
        //         p.Product,
        //         p.Quantity,
        //         p.Price)).ToList();

        //     purchase.ReplaceProducts(domainProducts);

        //     await _purchaseRepository.UpdateAsync(purchase, autoSave: true);
        // }

        // public async Task DeleteAsync(Guid id)
        // {
        //     await _purchaseRepository.DeleteAsync(id);
        // }
        public async Task UpdateAsync(Guid id, CreateUpdatePurchaseDto input)
        {
            if (input.Products == null || input.Products.Count == 0)
                throw new UserFriendlyException("Purchase must contain at least one product.");

            // 1️⃣ Load existing purchase WITH products
            var purchase = await _purchaseRepository.GetWithProductsAsync(id)
                ?? throw new UserFriendlyException("Purchase not found.");

            // 2️⃣ Create lookup for OLD products
            var oldProducts = purchase.Products.ToDictionary(
                p => (p.Product.ToLower(), p.Warehouse.ToLower()),
                p => p
            );

            // 3️⃣ Create lookup for NEW products
            var newProducts = input.Products.ToDictionary(
                p => (p.Product.ToLower(), p.Warehouse.ToLower()),
                p => p
            );

            // 4️⃣ Handle REMOVED & CHANGED products
            foreach (var oldItem in oldProducts)
            {
                if (!newProducts.TryGetValue(oldItem.Key, out var newItem))
                {
                    // ❌ Product removed → reduce full quantity
                    await ReduceStockAsync(
                        oldItem.Value.Product,
                        oldItem.Value.Warehouse,
                        oldItem.Value.Quantity
                    );
                }
                else
                {
                    // 🔄 Quantity difference
                    var diff = newItem.Quantity - oldItem.Value.Quantity;

                    if (diff > 0)
                    {
                        await IncreaseStockAsync(
                            oldItem.Value.Product,
                            oldItem.Value.Warehouse,
                            diff
                        );
                    }
                    else if (diff < 0)
                    {
                        await ReduceStockAsync(
                            oldItem.Value.Product,
                            oldItem.Value.Warehouse,
                            Math.Abs(diff)
                        );
                    }
                }
            }

            // 5️⃣ Handle NEWLY ADDED products
            foreach (var newItem in newProducts)
            {
                if (!oldProducts.ContainsKey(newItem.Key))
                {
                    await IncreaseStockAsync(
                        newItem.Value.Product,
                        newItem.Value.Warehouse,
                        newItem.Value.Quantity
                    );
                }
            }

            // 6️⃣ Update purchase main fields
            purchase.SetSupplierName(input.SupplierName);
            purchase.SetDateTime(input.DateTime);
            purchase.SetDiscount(input.Discount);
            purchase.SetPaidAmount(input.PaidAmount);

            // 7️⃣ Replace products in purchase
            var domainProducts = input.Products.Select(p =>
                new PurchaseProduct(
                    Guid.NewGuid(),
                    p.Warehouse,
                    p.Product,
                    p.Quantity,
                    p.Price
                )
            ).ToList();

            purchase.ReplaceProducts(domainProducts);

            // 8️⃣ Save
            await _purchaseRepository.UpdateAsync(purchase, autoSave: true);
        }
        private async Task IncreaseStockAsync(string product, string warehouse, int quantity)
        {
            if (quantity <= 0) return;

            var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                s.Product.ToLower() == product.ToLower() &&
                s.Warehouse.ToLower() == warehouse.ToLower()
            );

            if (stock == null)
            {
                await _stockRepository.InsertAsync(
                    new Stock(Guid.NewGuid(), product, warehouse, quantity)
                );
            }
            else
            {
                stock.Increase(quantity);
                await _stockRepository.UpdateAsync(stock);
            }
        }
        private async Task ReduceStockAsync(string product, string warehouse, int quantity)
        {
            if (quantity <= 0) return;

            var stock = await _stockRepository.FirstOrDefaultAsync(s =>
                s.Product.ToLower() == product.ToLower() &&
                s.Warehouse.ToLower() == warehouse.ToLower()
            );

            if (stock == null)
                return;

            stock.ReduceOrClear(quantity);

            if (stock.IsEmpty())
            {
                await _stockRepository.DeleteAsync(stock);
            }
            else
            {
                await _stockRepository.UpdateAsync(stock);
            }
        }

        // public async Task DeleteAsync(Guid id)
        // {
        //     var purchase = await _purchaseRepository.GetWithProductsAsync(id)
        //         ?? throw new UserFriendlyException("Purchase not found");
        //     // Logger.LogWarning($"Products count = {purchase.Products.Count}"); this shows producs count =0. 

        //     foreach (var product in purchase.Products)
        //     {
        //         var stock = await _stockRepository.FirstOrDefaultAsync(s =>
        //             s.Product.ToLower() == product.Product.ToLower() &&
        //             s.Warehouse.ToLower() == product.Warehouse.ToLower()
        //         );

        //         if (stock == null)
        //             continue;

        //         stock.Quantity -= product.Quantity;

        //         if (stock.Quantity <= 0)
        //         {
        //             await _stockRepository.DeleteAsync(stock);
        //         }
        //         else
        //         {
        //             await _stockRepository.UpdateAsync(stock);
        //         }
        //     }

        //     await _purchaseRepository.DeleteAsync(purchase);
        // }
        public async Task DeleteAsync(Guid id)
{
    var purchase = await _purchaseRepository.GetWithProductsAsync(id)
        ?? throw new UserFriendlyException("Purchase not found");

    foreach (var product in purchase.Products)
    {
        var stock = await _stockRepository.FirstOrDefaultAsync(s =>
            s.Product.ToLower() == product.Product.ToLower() &&
            s.Warehouse.ToLower() == product.Warehouse.ToLower()
        );

        if (stock == null)
            continue;

        // Reduce as much as possible
        var reduceQty = Math.Min(stock.Quantity, product.Quantity);

        stock.ReduceOrClear(reduceQty);

        if (stock.IsEmpty())
        {
            await _stockRepository.DeleteAsync(stock);
        }
        else
        {
            await _stockRepository.UpdateAsync(stock);
        }
    }

    await _purchaseRepository.DeleteAsync(purchase);
}

    }
}