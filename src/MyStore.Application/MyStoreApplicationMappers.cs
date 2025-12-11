using MyStore.Purchases;
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

[Mapper]
public partial class PurchaseProductToPurchaseProductDtoMapper : MapperBase<PurchaseProduct, PurchaseProductDto>
{
    // The required partial methods
    public override partial PurchaseProductDto Map(PurchaseProduct source);
    public override partial void Map(PurchaseProduct source, PurchaseProductDto destination);
}