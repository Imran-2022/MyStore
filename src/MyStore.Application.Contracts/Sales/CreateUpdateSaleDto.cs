using System;
using System.Collections.Generic;

namespace MyStore.Sales;

public class CreateUpdateSaleDto
{
    public string Customer { get; set; }
    public DateTime DateTime { get; set; }
    public List<CreateUpdateSaleProductDto> Products { get; set; }
}