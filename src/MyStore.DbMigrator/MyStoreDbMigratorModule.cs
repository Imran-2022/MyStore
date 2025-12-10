using MyStore.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyStore.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyStoreEntityFrameworkCoreModule),
    typeof(MyStoreApplicationContractsModule)
)]
public class MyStoreDbMigratorModule : AbpModule
{
}
