using System;
using Volo.Abp.Domain.Entities;

namespace MyStore.Purchases
{
    public class PurchaseProduct : Entity<Guid>
    {
        public string Warehouse { get; private set; }
        public Guid PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public string Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public decimal Total => Quantity * Price;

        private PurchaseProduct() { } // ORM

        public PurchaseProduct(Guid id, string warehouse, string product, int quantity, decimal price)
        {
            Id = id;
            Warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Quantity = quantity;
            Price = price;
        }
    }
}
