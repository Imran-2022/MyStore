using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace MyStore.Sales
{
    public class Sale : AggregateRoot<Guid>
    {
        public string Customer { get; set; }
        public DateTime DateTime { get; set; }
        public List<SaleProduct> Products { get; set; }

        protected Sale() { }

        public Sale(Guid id, string customer, DateTime dateTime, List<SaleProduct> products) : base(id)
        {
            Customer = customer;
            DateTime = dateTime;
            Products = products ?? new List<SaleProduct>();
        }
    }

    public class SaleProduct : Entity<Guid>
    {
        public string Warehouse { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        protected SaleProduct() { }

        public SaleProduct(Guid id, string warehouse, string product, int quantity, decimal price) : base(id)
        {
            Warehouse = warehouse;
            Product = product;
            Quantity = quantity;
            Price = price;
        }

        public decimal Total => Quantity * Price;
    }
}
