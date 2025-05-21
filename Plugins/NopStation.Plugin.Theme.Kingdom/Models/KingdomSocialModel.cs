using Nop.Web.Framework.Models;

namespace NopStation.Plugin.Theme.Kingdom.Models;

public partial record KingdomSocialModel : BaseNopModel
{
    public string InstagramLink { get; set; }
    public string PinterestLink { get; set; }
}
