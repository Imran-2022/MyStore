using System;
using Volo.Abp;
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
    public void Increase(int quantity)
    {
        if (quantity <= 0) return;
        Quantity += quantity;
    }
    public void ReduceOrClear(int quantity)
    {
        if (quantity <= 0) return;

        Quantity -= quantity;

        if (Quantity < 0)
            Quantity = 0;
    }
    public bool IsEmpty() => Quantity <= 0;
}