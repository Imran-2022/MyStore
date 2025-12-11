using System;

namespace MyStore.Purchases
{
    public class PurchaseDomainException : Exception
    {
        public PurchaseDomainException(string message) : base(message) { }

        public static PurchaseDomainException InvalidQuantity()
        {
            return new PurchaseDomainException("Quantity must be greater than zero.");
        }

        public static PurchaseDomainException InvalidPrice()
        {
            return new PurchaseDomainException("Price must be greater than zero.");
        }
    }
}
