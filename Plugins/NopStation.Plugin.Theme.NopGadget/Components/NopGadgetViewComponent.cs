using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Nop.Plugin.Themes.NopGadget.Components
{
    public class NopGadgetViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("~/Plugins/Theme.NopGadget/Views/Shared/_ThemeComponent.cshtml");
        }
    }
}
