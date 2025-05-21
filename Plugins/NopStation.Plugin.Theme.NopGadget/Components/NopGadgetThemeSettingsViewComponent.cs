using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using NopStation.Plugin.Theme.NopGadget.Domain;

namespace NopStation.Plugin.Theme.NopGadget.Components
{
    /// <summary>
    /// Represents NopGadget theme settings component
    /// </summary>
    // File: Components/NopGadgetThemeSettingsViewComponent.cs
    public class NopGadgetThemeSettingsViewComponent : NopViewComponent
    {
        private readonly NopGadgetSettings _nopGadgetSettings;

        public NopGadgetThemeSettingsViewComponent(NopGadgetSettings nopGadgetSettings)
        {
            _nopGadgetSettings = nopGadgetSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Plugins/NopStation.Plugin.Theme.NopGadget/Views/Shared/_ThemeStyles.cshtml", _nopGadgetSettings);
        }
    }
}