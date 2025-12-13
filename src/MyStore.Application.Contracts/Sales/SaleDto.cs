using System;
using System.Collections.Generic;

namespace MyStore.Sales;

public class SaleDto
{
    public Guid Id { get; set; }
    public string Customer { get; set; }
    public DateTime DateTime { get; set; }
    public List<SaleProductDto> Products { get; set; }
}