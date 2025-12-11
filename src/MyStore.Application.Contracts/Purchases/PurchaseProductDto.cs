using System;

namespace MyStore.Purchases;
public class PurchaseProductDto
{
    public Guid Id { get; set; }
    public string Warehouse { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    // public decimal Total { get; set; }
    public decimal Total => Quantity * Price; // calculated for convenience
}