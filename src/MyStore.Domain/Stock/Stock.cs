using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MyStore.Stocks
{
    public class Stock : AggregateRoot<Guid>
    {
        public string Product { get; private set; }
        public string Warehouse { get; private set; }
        public int Quantity { get; private set; }

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
            if (quantity <= 0)
                throw new BusinessException("Quantity must be positive");

            Quantity += quantity;
        }

        public void Reduce(int quantity)
        {
            if (quantity <= 0)
                throw new BusinessException("Quantity must be positive");

            Quantity -= quantity;
            if (Quantity < 0)
                Quantity = 0;
        }
        public void ReduceOrClear(int quantity)
        {
            Reduce(quantity);
        }
        public bool IsEmpty() => Quantity <= 0;
    }

}
