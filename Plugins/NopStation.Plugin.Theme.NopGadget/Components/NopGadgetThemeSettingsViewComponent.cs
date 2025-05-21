using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using NopStation.Plugin.Theme.NopGadget.Domain;

namespace NopStation.Plugin.Theme.NopGadget.Components
{
    /// <summary>
    /// Represents NopGadget theme settings component
    /// </summary>
    public class NopGadgetThemeSettingsViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;

        public NopGadgetThemeSettingsViewComponent(ISettingService settingService)
        {
            _settingService = settingService;
        }

        /// <summary>
        /// Invoke the component
        /// </summary>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Load theme settings
            var settings = await _settingService.LoadSettingAsync<NopGadgetSettings>();

            // Also set in ViewBag for backward compatibility
            ViewBag.ButtonColor = settings.ButtonColor;
            ViewBag.ThemeColor = settings.ThemeColor;

            // Return view with settings model
            return View("~/Plugins/NopStation.Plugin.Theme.NopGadget/Themes/NopGadget/Views/Shared/Components/NopGadgetThemeSettings/Default.cshtml", settings);
        }
    }
}