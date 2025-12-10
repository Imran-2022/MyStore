using Xunit;

namespace MyStore.EntityFrameworkCore;

[CollectionDefinition(MyStoreTestConsts.CollectionDefinitionName)]
public class MyStoreEntityFrameworkCoreCollection : ICollectionFixture<MyStoreEntityFrameworkCoreFixture>
{

}
