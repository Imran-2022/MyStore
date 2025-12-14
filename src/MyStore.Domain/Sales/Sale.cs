using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace MyStore.Sales
{
    public class Sale : AggregateRoot<Guid>
    {
        public string Customer { get; private set; }
        public DateTime DateTime { get; private set; }

        // Simple list, no backing field
        public List<SaleProduct> Products { get; private set; } = new();

        protected Sale() { } // ORM

        internal Sale(Guid id, string customer, DateTime dateTime, List<SaleProduct> products)
            : base(id)
        {
            Customer = !string.IsNullOrWhiteSpace(customer) ? customer
                : throw new ArgumentException("Customer cannot be empty.");
            DateTime = dateTime;

            if (products == null || products.Count == 0)
                throw new ArgumentException("Sale must have at least one product.");

            Products = products;
        }

        public void ReplaceProducts(List<SaleProduct> products)
        {
            if (products == null || products.Count == 0)
                throw new ArgumentException("Sale must have at least one product.");

            Products = products;
        }

        public void SetCustomer(string customer)
        {
            if (string.IsNullOrWhiteSpace(customer))
                throw new ArgumentException("Customer cannot be empty.");
            Customer = customer;
        }

        public void SetDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
}
