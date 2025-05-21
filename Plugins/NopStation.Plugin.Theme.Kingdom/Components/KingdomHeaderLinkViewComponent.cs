using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Media;
using NopStation.Plugin.Theme.Kingdom.Infrastructure.Cache;
using NopStation.Plugin.Theme.Kingdom.Models;

namespace NopStation.Plugin.Theme.Kingdom.Components;

public class KingdomHeaderLinkViewComponent : NopViewComponent
{
    #region Fields

    private readonly IWorkContext _workContext;
    private readonly IStaticCacheManager _cacheKeyService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly KingdomSettings _kingdomSettings;
    private readonly ILocalizationService _localizationService;
    private readonly IPictureService _pictureService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public KingdomHeaderLinkViewComponent(IWorkContext workContext,
        IStaticCacheManager cacheKeyService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        KingdomSettings kingdomSettings,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IWebHelper webHelper)
    {
        _workContext = workContext;
        _cacheKeyService = cacheKeyService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _kingdomSettings = kingdomSettings;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (!_kingdomSettings.EnableHeaderMenuOne && !_kingdomSettings.EnableHeaderMenuTwo)
            return Content("");

        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();

        var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheKey.TopMenuModelKey,
            store, language,
            _webHelper.IsCurrentConnectionSecured());

        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var model = new HeaderLinkModel();
            if (_kingdomSettings.EnableHeaderMenuOne)
            {
                var pictureUrl = await _pictureService.GetPictureUrlAsync(_kingdomSettings.HeaderMenuOneIconPictureId);
                var titleOne = await _localizationService.GetLocalizedSettingAsync(_kingdomSettings, x => x.HeaderMenuOneTitle, language.Id, store.Id);
                model.Link1 = new HeaderLinkModel.LinkModel
                {
                    Enabled = true,
                    Title = titleOne,
                    Link = await _localizationService.GetLocalizedSettingAsync(_kingdomSettings, x => x.HeaderMenuOneLink, language.Id, store.Id),
                    Icon = new PictureModel
                    {
                        ThumbImageUrl = pictureUrl,
                        FullSizeImageUrl = pictureUrl,
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("NopStation.Theme.Kingdom.Picture.AlterText"), titleOne),
                        Title = string.Format(await _localizationService.GetResourceAsync("NopStation.Theme.Kingdom.Picture.Title"), titleOne),
                    }
                };
            }
            if (_kingdomSettings.EnableHeaderMenuTwo)
            {
                var pictureUrl = await _pictureService.GetPictureUrlAsync(_kingdomSettings.HeaderMenuTwoIconPictureId);
                var titleTwo = await _localizationService.GetLocalizedSettingAsync(_kingdomSettings, x => x.HeaderMenuTwoTitle, language.Id, store.Id);
                model.Link2 = new HeaderLinkModel.LinkModel
                {
                    Enabled = true,
                    Title = titleTwo,
                    Link = await _localizationService.GetLocalizedSettingAsync(_kingdomSettings, x => x.HeaderMenuTwoLink, language.Id, store.Id),
                    Icon = new PictureModel
                    {
                        ThumbImageUrl = pictureUrl,
                        FullSizeImageUrl = pictureUrl,
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("NopStation.Theme.Kingdom.Picture.AlterText"), titleTwo),
                        Title = string.Format(await _localizationService.GetResourceAsync("NopStation.Theme.Kingdom.Picture.Title"), titleTwo),
                    }
                };
            }
            return model;
        });

        return View(cachedModel);
    }

    #endregion
}
