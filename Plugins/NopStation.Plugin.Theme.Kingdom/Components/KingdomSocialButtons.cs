using Microsoft.AspNetCore.Mvc;
using NopStation.Plugin.Misc.Core.Components;
using NopStation.Plugin.Theme.Kingdom.Models;

namespace NopStation.Plugin.Theme.Kingdom.Components;

public class KingdomSocialButtonsViewComponent : NopStationViewComponent
{
    #region Fields

    private readonly KingdomSettings _kingdomSettings;

    #endregion

    #region Ctor

    public KingdomSocialButtonsViewComponent(KingdomSettings kingdomSettings)
    {
        _kingdomSettings = kingdomSettings;
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        var model = new KingdomSocialModel()
        {
            InstagramLink = _kingdomSettings.InstagramLink,
            PinterestLink = _kingdomSettings.PinterestLink
        };

        return View(model);
    }

    #endregion
}
