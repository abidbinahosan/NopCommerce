using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Themes.NopGadget.Areas.Admin.Models;

namespace Nop.Plugin.Themes.NopGadget.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    public class ThemeEditorController : BasePluginController
    {
        public IActionResult Configure()
        {
            var model = new ThemeSettingsModel
            {
                PrimaryColor = "#007bff",
                FontFamily = "Roboto"
            };

            return View("~/Plugins/Theme.NopGadget/Areas/Admin/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ThemeSettingsModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            // Save logic here (e.g., save to settings or config file)

            //SuccessNotification("Theme settings saved successfully");
            return Configure();
        }
    }
}
