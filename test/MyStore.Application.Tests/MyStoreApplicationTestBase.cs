using Volo.Abp.Modularity;

namespace MyStore;

public abstract class MyStoreApplicationTestBase<TStartupModule> : MyStoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
