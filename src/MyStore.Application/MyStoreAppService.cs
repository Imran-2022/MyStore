using MyStore.Localization;
using Volo.Abp.Application.Services;

namespace MyStore;

/* Inherit your application services from this class.
 */
public abstract class MyStoreAppService : ApplicationService
{
    protected MyStoreAppService()
    {
        LocalizationResource = typeof(MyStoreResource);
    }
}
