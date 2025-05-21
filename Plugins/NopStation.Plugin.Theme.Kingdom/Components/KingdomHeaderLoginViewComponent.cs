using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace NopStation.Plugin.Theme.Kingdom.Components;

public class KingdomHeaderLoginViewComponent : NopViewComponent
{
    #region Fields

    private readonly KingdomSettings _kingdomSettings;
    private readonly ICustomerModelFactory _customerModelFactory;

    #endregion

    #region Ctor

    public KingdomHeaderLoginViewComponent(KingdomSettings kingdomSettings,
        ICustomerModelFactory customerModelFactory)
    {
        _kingdomSettings = kingdomSettings;
        _customerModelFactory = customerModelFactory;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (!_kingdomSettings.EnableLoginBoxAtHeader)
            return Content("");

        var model = await _customerModelFactory.PrepareLoginModelAsync(null);
        return View(model);
    }

    #endregion
}
