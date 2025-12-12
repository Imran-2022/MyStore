using MyStore.Purchases;
using MyStore.Stocks;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace MyStore;

/*
 * You can add your own mappings here.
 * [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
 * public partial class MyStoreApplicationMappers : MapperBase<BookDto, CreateUpdateBookDto>
 * {
 *    public override partial CreateUpdateBookDto Map(BookDto source);
 * 
 *    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
 * }
 */
[Mapper]
public partial class PurchaseToPurchaseDtoMapper : MapperBase<Purchase, PurchaseDto>
{
    public override partial PurchaseDto Map(Purchase source);
    public override partial void Map(Purchase source, PurchaseDto destination);
}

// it works, without bellow code however. 
[Mapper]
public partial class PurchaseProductToPurchaseProductDtoMapper : MapperBase<PurchaseProduct, PurchaseProductDto>
{
    // The required partial methods
    public override partial PurchaseProductDto Map(PurchaseProduct source);
    public override partial void Map(PurchaseProduct source, PurchaseProductDto destination);
}

[Mapper]
public partial class StockMapper : MapperBase<Stock, StockDto>
{
    public override partial StockDto Map(Stock source);
    public override partial void Map(Stock source, StockDto destination);
}