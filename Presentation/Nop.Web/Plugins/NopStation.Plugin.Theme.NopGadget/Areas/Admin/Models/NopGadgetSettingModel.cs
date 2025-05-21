using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopStation.Plugin.Theme.NopGadget.Areas.Admin.Models
{
    public record NopGadgetSettingModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.NopGadget.ThemeColor")]
        public string ThemeColor { get; set; }

        [NopResourceDisplayName("Plugins.NopGadget.EnableCustomBanner")]
        public bool EnableCustomBanner { get; set; }

        [NopResourceDisplayName("Plugins.NopGadget.ButtonColor")]
        public string ButtonColor { get; set; }
    }
}
