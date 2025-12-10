using Volo.Abp.Modularity;

namespace MyStore;

[DependsOn(
    typeof(MyStoreDomainModule),
    typeof(MyStoreTestBaseModule)
)]
public class MyStoreDomainTestModule : AbpModule
{

}
