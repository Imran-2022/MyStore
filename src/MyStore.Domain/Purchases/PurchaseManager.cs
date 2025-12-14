using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp;
using MyStore.Stocks;

namespace MyStore.Purchases
{
    public class PurchaseManager : DomainService
    {
        private readonly StockManager _stockManager;

        public PurchaseManager(StockManager stockManager)
        {
            _stockManager = stockManager;
        }

        // Create purchase with stock increase
        public async Task<Purchase> CreatePurchaseAsync(
            string purchaseCode,
            string supplierName,
            DateTime dateTime,
            List<PurchaseProduct> products,
            decimal discount = 0,
            decimal paidAmount = 0
        )
        {
            if (products == null || products.Count == 0)
                throw new BusinessException("Purchase must contain at least one product");

            foreach (var p in products)
            {
                if (p.Quantity <= 0) throw PurchaseDomainException.InvalidQuantity();
                if (p.Price <= 0) throw PurchaseDomainException.InvalidPrice();
            }

            var purchase = new Purchase(
                purchaseCode,
                supplierName,
                dateTime,
                products,
                discount,
                paidAmount
            );

            // Increase stock
            foreach (var p in products)
            {
                await _stockManager.IncreaseAsync(p.Product, p.Warehouse, p.Quantity);
            }

            return purchase;
        }

        // Update purchase (stock adjustment)
        public async Task UpdatePurchaseAsync(Purchase purchase, List<PurchaseProduct> newProducts, string supplierName, DateTime dateTime, decimal discount, decimal paidAmount)
        {
            if (purchase == null) throw new ArgumentNullException(nameof(purchase));
            if (newProducts == null || newProducts.Count == 0)
                throw new BusinessException("Purchase must contain at least one product");

            var oldProducts = purchase.Products.ToDictionary(
                p => (p.Product.ToLower(), p.Warehouse.ToLower()), p => p
            );

            var updatedProducts = newProducts.ToDictionary(
                p => (p.Product.ToLower(), p.Warehouse.ToLower()), p => p
            );

            // Adjust stock
            foreach (var oldItem in oldProducts)
            {
                if (!updatedProducts.TryGetValue(oldItem.Key, out var newItem))
                {
                    // Removed product → reduce stock
                    await _stockManager.ReduceAsync(oldItem.Value.Product, oldItem.Value.Warehouse, oldItem.Value.Quantity);
                }
                else
                {
                    var diff = newItem.Quantity - oldItem.Value.Quantity;
                    if (diff > 0)
                        await _stockManager.IncreaseAsync(oldItem.Value.Product, oldItem.Value.Warehouse, diff);
                    else if (diff < 0)
                        await _stockManager.ReduceAsync(oldItem.Value.Product, oldItem.Value.Warehouse, -diff);
                }
            }

            // Newly added products
            foreach (var updated in updatedProducts)
            {
                if (!oldProducts.ContainsKey(updated.Key))
                {
                    await _stockManager.IncreaseAsync(updated.Value.Product, updated.Value.Warehouse, updated.Value.Quantity);
                }
            }

            // Update main fields
            purchase.SetSupplierName(supplierName);
            purchase.SetDateTime(dateTime);
            purchase.SetDiscount(discount);
            purchase.SetPaidAmount(paidAmount);

            // Replace products
            purchase.ReplaceProducts(newProducts);
        }

        // Delete purchase (reduce stock)
        public async Task DeletePurchaseAsync(Purchase purchase)
        {
            if (purchase == null) return;

            foreach (var p in purchase.Products)
            {
                await _stockManager.ReduceAsync(p.Product, p.Warehouse, p.Quantity);
            }
        }
    }
}
