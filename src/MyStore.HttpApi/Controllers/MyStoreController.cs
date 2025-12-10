using MyStore.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyStore.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyStoreController : AbpControllerBase
{
    protected MyStoreController()
    {
        LocalizationResource = typeof(MyStoreResource);
    }
}
