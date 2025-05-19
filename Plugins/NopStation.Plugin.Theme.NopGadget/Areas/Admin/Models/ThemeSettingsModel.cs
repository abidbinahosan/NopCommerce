using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Themes.NopGadget.Areas.Admin.Models
{
    public record ThemeSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Theme.NopGadget.PrimaryColor")]
        public string PrimaryColor { get; set; }

        [NopResourceDisplayName("Plugins.Theme.NopGadget.FontFamily")]
        public string FontFamily { get; set; }
    }
}
