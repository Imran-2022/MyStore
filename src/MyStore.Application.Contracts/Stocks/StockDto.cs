using System;
using Volo.Abp.Application.Dtos;

namespace MyStore.Stocks
{
    public class StockDto : EntityDto<Guid>
    {
        public string Product { get; set; }
        public string Warehouse { get; set; }
        public int Quantity { get; set; }
    }
}
