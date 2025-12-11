using System;
using System.Collections.Generic;

namespace MyStore.Purchases;
// DTO for Purchase
public class PurchaseDto
{
    public Guid Id { get; set; }
    public string PurchaseCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime DateTime { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal PayableAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public List<PurchaseProductDto> Products { get; set; } = new(); // initialize list
}
