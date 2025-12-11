namespace MyStore.Purchases
{
    public class CreateUpdatePurchaseProductDto
    {
        public string Warehouse { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
