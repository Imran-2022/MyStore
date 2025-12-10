using MyStore.Samples;
using Xunit;

namespace MyStore.EntityFrameworkCore.Applications;

[Collection(MyStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyStoreEntityFrameworkCoreTestModule>
{

}
