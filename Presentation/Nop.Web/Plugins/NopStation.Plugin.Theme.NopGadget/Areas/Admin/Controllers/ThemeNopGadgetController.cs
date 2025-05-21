using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using NopStation.Plugin.Theme.NopGadget.Areas.Admin.Models;
using NopStation.Plugin.Theme.NopGadget.Domain;

namespace NopStation.Plugin.Theme.NopGadget.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public class ThemeNopGadgetController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;

        public ThemeNopGadgetController(ISettingService settingService, INotificationService notificationService)
        {
            _settingService = settingService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Configure()
        {
            var settings = await _settingService.LoadSettingAsync<NopGadgetSettings>();
            var model = new ConfigurationModel
            {
                ThemeColor = settings.ThemeColor,
                EnableCustomBanner = settings.EnableCustomBanner,
                ButtonColor = settings.ButtonColor
            };
            ViewData["ActiveMenuSystemName"] = "NopGadget.Configuration"; // for menu highlighting
            return View("~/Plugins/NopStation.Plugin.Theme.NopGadget/Areas/Admin/Views/ThemeNopGadget/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var settings = await _settingService.LoadSettingAsync<NopGadgetSettings>();
            settings.ThemeColor = model.ThemeColor;
            settings.EnableCustomBanner = model.EnableCustomBanner;
            settings.ButtonColor = model.ButtonColor;

            await _settingService.SaveSettingAsync(settings);

            // Critical: Clear cache after saving settings
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification("Settings saved successfully");
            return await Configure();
        }
    }
}