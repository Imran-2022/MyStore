using System;
using Volo.Abp.Domain.Entities;
namespace MyStore.Stocks;

public class Stock : AggregateRoot<Guid>
{
    // Name of the product
    public string Product { get; set; }

    // Warehouse name
    public string Warehouse { get; set; }

    // Quantity available
    public int Quantity { get; set; }

    // Constructor for EF Core
    protected Stock() { }

    public Stock(Guid id, string product, string warehouse, int quantity)
        : base(id)
    {
        Product = product;
        Warehouse = warehouse;
        Quantity = quantity;
    }
}