using System;
using System.Collections.Generic;

namespace MyStore.Purchases
{
    public class CreateUpdatePurchaseDto
    {
        public string PurchaseCode { get; set; } // required from frontend
        public string SupplierName { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidAmount { get; set; }
        public List<CreateUpdatePurchaseProductDto> Products { get; set; } = new();
    }
}
