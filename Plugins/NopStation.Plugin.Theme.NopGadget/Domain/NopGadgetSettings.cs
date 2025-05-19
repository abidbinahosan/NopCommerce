using Nop.Core;

namespace NopStation.Plugin.Theme.NopGadget.Domain
{
    public class NopGadgetSettings : BaseEntity
    {
        public string PrimaryColor { get; set; }
        public string FontFamily { get; set; }
    }
}
