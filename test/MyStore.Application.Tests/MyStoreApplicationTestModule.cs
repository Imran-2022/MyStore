using Volo.Abp.Modularity;

namespace MyStore;

[DependsOn(
    typeof(MyStoreApplicationModule),
    typeof(MyStoreDomainTestModule)
)]
public class MyStoreApplicationTestModule : AbpModule
{

}
