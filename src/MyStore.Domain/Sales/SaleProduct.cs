using System;
using Volo.Abp.Domain.Entities;

namespace MyStore.Sales
{
    public class SaleProduct : Entity<Guid>
    {
        public Guid SaleId { get; set; }       // FK
        public Sale Sale { get; set; }         // navigation

        public string Warehouse { get; private set; }
        public string Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        private SaleProduct() { } // ORM

        public SaleProduct(Guid id, string warehouse, string product, int quantity, decimal price)
            : base(id)
        {
            Warehouse = !string.IsNullOrWhiteSpace(warehouse) ? warehouse
                : throw new ArgumentException("Warehouse cannot be empty.");
            Product = !string.IsNullOrWhiteSpace(product) ? product
                : throw new ArgumentException("Product cannot be empty.");
            Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be > 0.");
            Price = price >= 0 ? price : throw new ArgumentException("Price cannot be negative.");
        }
    }
}
