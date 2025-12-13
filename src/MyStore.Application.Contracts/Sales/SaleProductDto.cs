using System;
using System.Collections.Generic;

namespace MyStore.Sales;

public class SaleProductDto
{
     public Guid Id { get; set; } 
    public string Warehouse { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}