using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MyStore.Stocks;

public class Stock : AggregateRoot<Guid>
{
    public string Product { get; private set; }
    public string Warehouse { get; private set; }
    public int Quantity { get; private set; }

    // Only domain services (StockManager) can create Stock
    internal Stock(Guid id, string product, string warehouse, int quantity)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(product)) throw new BusinessException("Stock:ProductRequired");
        if (string.IsNullOrWhiteSpace(warehouse)) throw new BusinessException("Stock:WarehouseRequired");
        if (quantity < 0) throw new BusinessException("Stock:QuantityCannotBeNegative");

        Product = product;
        Warehouse = warehouse;
        Quantity = quantity;
    }

    internal void Increase(int quantity)
    {
        if (quantity <= 0) throw new BusinessException("Stock:QuantityMustBeGreaterThanZero");
        Quantity += quantity;
    }

    internal void Reduce(int quantity)
    {
        if (quantity <= 0) throw new BusinessException("Stock:QuantityMustBeGreaterThanZero");

        if (Quantity < quantity)
            throw new BusinessException("Stock:NotEnoughStock");

        Quantity -= quantity;
    }

    internal void ReduceOrClear(int quantity)
    {
        Reduce(quantity);
    }
    public bool IsEmpty() => Quantity <= 0;

}
