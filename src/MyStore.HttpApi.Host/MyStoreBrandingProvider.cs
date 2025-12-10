using Microsoft.Extensions.Localization;
using MyStore.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MyStore;

[Dependency(ReplaceServices = true)]
public class MyStoreBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyStoreResource> _localizer;

    public MyStoreBrandingProvider(IStringLocalizer<MyStoreResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
