using Volo.Abp.Modularity;

namespace MyStore;

/* Inherit from this class for your domain layer tests. */
public abstract class MyStoreDomainTestBase<TStartupModule> : MyStoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
