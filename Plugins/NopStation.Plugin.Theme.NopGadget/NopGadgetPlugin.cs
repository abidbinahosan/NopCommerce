using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Themes.NopGadget
{
    public class NopGadgetPlugin : BasePlugin
    {
        private readonly ILocalizationService _localizationService;

        public NopGadgetPlugin(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Themes.NopGadget.Settings.PrimaryColor"] = "Primary Color",
                ["Plugins.Themes.NopGadget.Settings.FontFamily"] = "Font Family"
            });

            await base.InstallAsync(); // This will trigger migration automatically
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Themes.NopGadget.Settings.PrimaryColor");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Themes.NopGadget.Settings.FontFamily");

            await base.UninstallAsync();
        }
    }
}
