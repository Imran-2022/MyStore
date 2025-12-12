namespace MyStore.Purchases
{
    public class CreateUpdatePurchaseProductDto
    {
        public string Warehouse { get; set; }
        public string Product { get; set; }
        //  public string Warehouse { get; set; } = string.Empty;
        // public string Product { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
