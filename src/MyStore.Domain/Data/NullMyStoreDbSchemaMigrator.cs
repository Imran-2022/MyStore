using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyStore.Data;

/* This is used if database provider does't define
 * IMyStoreDbSchemaMigrator implementation.
 */
public class NullMyStoreDbSchemaMigrator : IMyStoreDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
