using Nop.Core.Configuration;

namespace NopStation.Plugin.Theme.NopGadget.Services
{
    public class NopGadgetThemeSettings : ISettings
    {
        public string ThemeColor { get; set; }
        public bool EnableCustomBanner { get; set; }
        public string ButtonColor { get; set; }
    }
}
