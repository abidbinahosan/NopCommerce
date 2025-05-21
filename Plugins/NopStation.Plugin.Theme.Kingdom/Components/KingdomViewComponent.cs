using Microsoft.AspNetCore.Mvc;
using NopStation.Plugin.Misc.Core.Components;

namespace NopStation.Plugin.Theme.Kingdom.Components;

public class KingdomViewComponent : NopStationViewComponent
{
    public IViewComponentResult Invoke(string widgetZone)
    {
        return View();
    }
}
