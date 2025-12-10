using System.Threading.Tasks;

namespace MyStore.Data;

public interface IMyStoreDbSchemaMigrator
{
    Task MigrateAsync();
}
