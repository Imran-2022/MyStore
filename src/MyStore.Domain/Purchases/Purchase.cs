using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace MyStore.Purchases;

public class Purchase : AggregateRoot<Guid>   // Aggregate Root
{
    public DateTime DateTime { get; private set; }
    public string PurchaseCode { get; private set; }
    public string SupplierName { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal Discount { get; private set; }
    public decimal PayableAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal DueAmount { get; private set; }

    public List<PurchaseProduct> Products { get; private set; } = new();

    private Purchase() { } // For ORM

    internal Purchase(
        string purchaseCode,
        string supplierName,
        DateTime dateTime,
        List<PurchaseProduct> products,
        decimal discount = 0,
        decimal paidAmount = 0)
    {
        PurchaseCode = purchaseCode ?? throw new ArgumentNullException(nameof(purchaseCode));
        SupplierName = supplierName ?? throw new ArgumentNullException(nameof(supplierName));
        DateTime = dateTime;
        Products = products ?? throw new ArgumentNullException(nameof(products));
        Discount = discount;
        PaidAmount = paidAmount;

        CalculateTotals();
    }

    internal void CalculateTotals()
    {
        TotalAmount = 0;
        foreach (var item in Products)
        {
            TotalAmount += item.Total;
        }

        PayableAmount = TotalAmount - Discount;
        DueAmount = PayableAmount - PaidAmount;
        if (DueAmount < 0) DueAmount = 0;

        /*
        // Domain calculates derived fields
        TotalAmount = products.Sum(p => p.Quantity * p.Price);
        PayableAmount = TotalAmount - discount;
        DueAmount = PayableAmount - paidAmount;
        */
    }

    internal void AddProduct(PurchaseProduct product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        Products.Add(product);
        CalculateTotals();
    }
    // Domain methods for safe updates
    internal void SetSupplierName(string supplierName)
    {
        if (string.IsNullOrWhiteSpace(supplierName))
            throw new ArgumentException("Supplier name cannot be empty.", nameof(supplierName));

        SupplierName = supplierName;
    }

    internal void SetDateTime(DateTime dateTime)
    {
        DateTime = dateTime;
    }

    internal void SetDiscount(decimal discount)
    {
        if (discount < 0) throw new ArgumentException("Discount cannot be negative.", nameof(discount));
        Discount = discount;
        CalculateTotals();
    }

    internal void SetPaidAmount(decimal paidAmount)
    {
        if (paidAmount < 0) throw new ArgumentException("Paid amount cannot be negative.", nameof(paidAmount));
        PaidAmount = paidAmount;
        CalculateTotals();
    }

    internal void ReplaceProducts(List<PurchaseProduct> products)
    {
        if (products == null || products.Count == 0)
            throw new ArgumentException("Purchase must have at least one product.", nameof(products));

        Products.Clear();
        foreach (var p in products)
        {
            if (p.Quantity <= 0) throw new ArgumentException("Product quantity must be greater than zero.");
            if (p.Price < 0) throw new ArgumentException("Product price cannot be negative.");
            Products.Add(p);
        }

        CalculateTotals();
    }
    // Optional: allow explicit setting of purchase code (if needed)
    internal void SetPurchaseCode(string purchaseCode)
    {
        if (string.IsNullOrWhiteSpace(purchaseCode))
            throw new ArgumentException("PurchaseCode cannot be empty.", nameof(purchaseCode));
        PurchaseCode = purchaseCode;
    }
}
