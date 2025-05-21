using Nop.Core;
using Nop.Core.Configuration;

namespace NopStation.Plugin.Theme.NopGadget.Domain
{
    public class NopGadgetSettings : BaseEntity, ISettings
    {
        public int Id { get; set; }

        // Example setting: theme primary color (you can add more settings here)
        public string ThemeColor { get; set; }

        // Example toggle setting
        public bool EnableCustomBanner { get; set; }
        public string ButtonColor { get; set; }
    }
}
