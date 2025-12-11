using System;
using System.Collections.Generic;

namespace MyStore.Purchases
{
    public class PurchaseManager
    {
        public Purchase CreatePurchase(
            string supplierName,
            DateTime dateTime,
            List<PurchaseProduct> products,
            decimal discount = 0,
            decimal paidAmount = 0)
        {
            if (products == null || products.Count == 0)
            {
                throw new PurchaseDomainException("Purchase must have at least one product.");
            }

            foreach (var p in products)
            {
                if (p.Quantity <= 0) throw PurchaseDomainException.InvalidQuantity();
                if (p.Price <= 0) throw PurchaseDomainException.InvalidPrice();
            }

            var purchaseCode = GeneratePurchaseCode(dateTime);

            var purchase = new Purchase(
                purchaseCode,
                supplierName,
                dateTime,
                products,
                discount,
                paidAmount
            );

            return purchase;
        }

        private string GeneratePurchaseCode(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss"); // e.g., 20251211122128
        }
    }
}
