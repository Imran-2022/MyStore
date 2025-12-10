using MyStore.Samples;
using Xunit;

namespace MyStore.EntityFrameworkCore.Domains;

[Collection(MyStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyStoreEntityFrameworkCoreTestModule>
{

}
