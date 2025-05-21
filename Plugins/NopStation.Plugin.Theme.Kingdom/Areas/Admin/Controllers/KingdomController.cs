using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using NopStation.Plugin.Misc.Core.Controllers;
using NopStation.Plugin.Misc.Core.Filters;
using NopStation.Plugin.Theme.Kingdom.Areas.Admin.Models;
using NopStation.Plugin.Theme.Kingdom.Infrastructure.Cache;

namespace NopStation.Plugin.Theme.Kingdom.Areas.Admin.Controllers;

public class KingdomController : NopStationAdminController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;
    private readonly IStoreService _storeService;
    private readonly INopFileProvider _nopFileProvider;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly ILanguageService _languageService;

    #endregion

    #region Ctor

    public KingdomController(IPermissionService permissionService,
ISettingService settingService,
IStoreContext storeContext,
ILocalizationService localizationService,
INotificationService notificationService,
ILogger logger,
IStoreService storeService,
INopFileProvider nopFileProvider,
IStaticCacheManager staticCacheManager,
ILanguageService languageService)
    {
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _logger = logger;
        _storeService = storeService;
        _nopFileProvider = nopFileProvider;
        _staticCacheManager = staticCacheManager;
        _languageService = languageService;
    }

    #endregion

    #region Utilities

    protected async Task UpdateCssFilesAsync(int storeId)
    {
        if (storeId == 0)
            foreach (var store in await _storeService.GetAllStoresAsync())
                await UpdateCssFileAsync(store.Id);
        else
            await UpdateCssFileAsync(storeId);
    }

    protected async Task UpdateCssFileAsync(int storeId)
    {
        var cssText = await GetThemeColorCssTextAsync(storeId);

        var path = $"~/Themes/Kingdom/Content/css/styles.default-{storeId}.css";
        var fileSystemPath = _nopFileProvider.MapPath(path);

        try
        {
            if (!_nopFileProvider.FileExists(fileSystemPath))
                _nopFileProvider.CreateFile(fileSystemPath);

            await _nopFileProvider.WriteAllTextAsync(fileSystemPath, cssText, Encoding.UTF8);
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync($"Could not save or create {fileSystemPath} ", e);
        }
    }

    protected async Task<string> GetThemeColorCssTextAsync(int storeId)
    {
        var kingdomSetting = await _settingService.LoadSettingAsync<KingdomSettings>(storeId);

        var cssText = kingdomSetting.CustomCss + Environment.NewLine +
            ":root { " + Environment.NewLine +
            "   --primary-color: " + kingdomSetting.PrimaryThemeColor + ";" + Environment.NewLine +
            "   --secondary-color: " + kingdomSetting.SecondaryThemeColor + ";" + Environment.NewLine +
            "   --category-even: " + kingdomSetting.CategoryEvenColor + ";" + Environment.NewLine +
            "   --category-odd: " + kingdomSetting.CategoryOddColor + ";" + Environment.NewLine +
            "}" +
            Environment.NewLine;

        return cssText;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(KingdomPermissionProvider.ManageKingdom))
            return AccessDeniedView();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var kingdomSettings = await _settingService.LoadSettingAsync<KingdomSettings>(storeId);
        var model = kingdomSettings.ToSettingsModel<ConfigurationModel>();

        //locales
        await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
        {
            locale.FooterContactUsText = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterContactUsText, languageId, 0, false, false);

            locale.HeaderMenuOneTitle = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.HeaderMenuOneTitle, languageId, 0, false, false);

            locale.HeaderMenuTwoTitle = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.HeaderMenuTwoTitle, languageId, 0, false, false);

            locale.FooterDescriptionBoxOneTitle = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxOneTitle, languageId, 0, false, false);
            locale.FooterDescriptionBoxOneText = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxOneText, languageId, 0, false, false);
            locale.FooterDescriptionBoxOneUrl = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxOneUrl, languageId, 0, false, false);

            locale.FooterDescriptionBoxTwoTitle = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoTitle, languageId, 0, false, false);
            locale.FooterDescriptionBoxTwoText = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoText, languageId, 0, false, false);
            locale.FooterDescriptionBoxTwoUrl = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoUrl, languageId, 0, false, false);

            locale.FooterDescriptionBoxThreeTitle = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeTitle, languageId, 0, false, false);
            locale.FooterDescriptionBoxThreeText = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeText, languageId, 0, false, false);
            locale.FooterDescriptionBoxThreeUrl = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeUrl, languageId, 0, false, false);

            locale.FooterDescriptionBoxFourTitle = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxFourTitle, languageId, 0, false, false);
            locale.FooterDescriptionBoxFourText = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxFourText, languageId, 0, false, false);
            locale.FooterDescriptionBoxFourUrl = await _localizationService
                .GetLocalizedSettingAsync(kingdomSettings, x => x.FooterDescriptionBoxFourUrl, languageId, 0, false, false);
        });

        model.ActiveStoreScopeConfiguration = storeId;

        if (storeId > 0)
        {
            #region Color

            model.CategoryEvenColor_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CategoryEvenColor, storeId);
            model.CategoryOddColor_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CategoryOddColor, storeId);
            model.PrimaryThemeColor_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.PrimaryThemeColor, storeId);
            model.SecondaryThemeColor_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.SecondaryThemeColor, storeId);

            #endregion

            #region General 

            model.PhoneNumber_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.PhoneNumber, storeId);
            model.InstagramLink_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.InstagramLink, storeId);
            model.PinterestLink_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.PinterestLink, storeId);

            model.FooterContactUsText_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterContactUsText, storeId);
            model.HideDesignedByNopStationAtFooter_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HideDesignedByNopStationAtFooter, storeId);
            model.ShowLogoAtFooter_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.ShowLogoAtFooter, storeId);
            model.FooterLogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterLogoPictureId, storeId);
            model.ShowSupportedCardsAtFooter_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.ShowSupportedCardsAtFooter, storeId);
            model.FooterSupportedCardsPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterSupportedCardsPictureId, storeId);
            model.ShowSupportedCardsInProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.ShowSupportedCardsInProductDetailsPage, storeId);
            model.ProductDetailsPageSupportedCardsPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.ProductDetailsPageSupportedCardsPictureId, storeId);

            model.EnableLoginBoxAtHeader_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableLoginBoxAtHeader, storeId);
            model.EnableImageLazyLoad_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableImageLazyLoad, storeId);
            model.LazyLoadPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.LazyLoadPictureId, storeId);
            model.HideReviewInProductBox_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HideReviewInProductBox, storeId);
            model.EnableStickyHeader_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableStickyHeader, storeId);
            model.HideProductBoxButtonsOnMobile_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HideProductBoxButtonsOnMobile, storeId);
            model.NuberOfItemsToShowInProductFilters_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.NuberOfItemsToShowInProductFilters, storeId);
            model.CustomCss_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CustomCss, storeId);

            #endregion

            #region Carousels    

            model.EnableCarouselsForDefaultEntityLists_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableCarouselsForDefaultEntityLists, storeId);
            model.EnableCarouselAutoPlay_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableCarouselAutoPlay, storeId);
            model.CarouselAutoPlayTimeout_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CarouselAutoPlayTimeout, storeId);
            model.CarouselAutoPlayHoverPause_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CarouselAutoPlayHoverPause, storeId);
            model.EnableCarouselLoop_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableCarouselLoop, storeId);
            model.EnableCarouselNavigation_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableCarouselNavigation, storeId);
            model.EnableCarouselPagination_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableCarouselPagination, storeId);
            model.CarouselPaginationClickable_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CarouselPaginationClickable, storeId);
            model.CarouselPaginationTypeId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CarouselPaginationTypeId, storeId);
            model.CarouselPaginationDynamicBullets_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CarouselPaginationDynamicBullets, storeId);
            model.CarouselPaginationDynamicMainBullets_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.CarouselPaginationDynamicMainBullets, storeId);

            #endregion

            #region Home page        

            model.HideHomePageBestSellers_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HideHomePageBestSellers, storeId);
            model.HideHomePageFeaturedCategories_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HideHomePageFeaturedCategories, storeId);
            model.HideHomePageProducts_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HideHomePageProducts, storeId);

            #endregion

            #region Header   

            model.EnableHeaderMenuOne_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableHeaderMenuOne, storeId);
            model.HeaderMenuOneTitle_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HeaderMenuOneTitle, storeId);
            model.HeaderMenuOneIconPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HeaderMenuOneIconPictureId, storeId);
            model.HeaderMenuOneLink_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HeaderMenuOneLink, storeId);

            model.EnableHeaderMenuTwo_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableHeaderMenuTwo, storeId);
            model.HeaderMenuTwoTitle_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HeaderMenuTwoTitle, storeId);
            model.HeaderMenuTwoIconPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HeaderMenuTwoIconPictureId, storeId);
            model.HeaderMenuTwoLink_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.HeaderMenuTwoLink, storeId);

            #endregion

            #region Footer description

            model.EnableFooterDescriptionBoxOne_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxOne, storeId);
            model.FooterDescriptionBoxOneTitle_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxOneTitle, storeId);
            model.FooterDescriptionBoxOneText_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxOneText, storeId);
            model.FooterDescriptionBoxOneUrl_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxOneUrl, storeId);
            model.FooterDescriptionBoxOnePictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxOnePictureId, storeId);

            model.EnableFooterDescriptionBoxTwo_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxTwo, storeId);
            model.FooterDescriptionBoxTwoTitle_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoTitle, storeId);
            model.FooterDescriptionBoxTwoText_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoText, storeId);
            model.FooterDescriptionBoxTwoUrl_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoUrl, storeId);
            model.FooterDescriptionBoxTwoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoPictureId, storeId);

            model.EnableFooterDescriptionBoxThree_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxThree, storeId);
            model.FooterDescriptionBoxThreeTitle_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeTitle, storeId);
            model.FooterDescriptionBoxThreeText_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeText, storeId);
            model.FooterDescriptionBoxThreeUrl_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeUrl, storeId);
            model.FooterDescriptionBoxThreePictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxThreePictureId, storeId);

            model.EnableFooterDescriptionBoxFour_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxFour, storeId);
            model.FooterDescriptionBoxFourTitle_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxFourTitle, storeId);
            model.FooterDescriptionBoxFourText_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxFourText, storeId);
            model.FooterDescriptionBoxFourUrl_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxFourUrl, storeId);
            model.FooterDescriptionBoxFourPictureId_OverrideForStore = await _settingService.SettingExistsAsync(kingdomSettings, x => x.FooterDescriptionBoxFourPictureId, storeId);

            #endregion
        }

        return View(model);
    }

    [EditAccess, HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(KingdomPermissionProvider.ManageKingdom))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var kingdomSettings = await _settingService.LoadSettingAsync<KingdomSettings>(storeScope);
        kingdomSettings = model.ToSettings(kingdomSettings);

        #region Color

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CategoryEvenColor, model.CategoryEvenColor_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CategoryOddColor, model.CategoryOddColor_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.PrimaryThemeColor, model.PrimaryThemeColor_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.SecondaryThemeColor, model.SecondaryThemeColor_OverrideForStore, storeScope, false);

        #endregion

        #region General

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.PhoneNumber, model.PhoneNumber_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.InstagramLink, model.InstagramLink_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.PinterestLink, model.PinterestLink_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterContactUsText, model.FooterContactUsText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HideDesignedByNopStationAtFooter, model.HideDesignedByNopStationAtFooter_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.ShowLogoAtFooter, model.ShowLogoAtFooter_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterLogoPictureId, model.FooterLogoPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.ShowSupportedCardsAtFooter, model.ShowSupportedCardsAtFooter_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterSupportedCardsPictureId, model.FooterSupportedCardsPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.ShowSupportedCardsInProductDetailsPage, model.ShowSupportedCardsInProductDetailsPage_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.ProductDetailsPageSupportedCardsPictureId, model.ProductDetailsPageSupportedCardsPictureId_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableLoginBoxAtHeader, model.EnableLoginBoxAtHeader_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableImageLazyLoad, model.EnableImageLazyLoad_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.LazyLoadPictureId, model.LazyLoadPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HideReviewInProductBox, model.HideReviewInProductBox_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableStickyHeader, model.EnableStickyHeader_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HideProductBoxButtonsOnMobile, model.HideProductBoxButtonsOnMobile_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.NuberOfItemsToShowInProductFilters, model.NuberOfItemsToShowInProductFilters_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CustomCss, model.CustomCss_OverrideForStore, storeScope, false);

        #endregion

        #region Carousels

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableCarouselsForDefaultEntityLists, model.EnableCarouselsForDefaultEntityLists_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableCarouselAutoPlay, model.EnableCarouselAutoPlay_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CarouselAutoPlayTimeout, model.CarouselAutoPlayTimeout_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CarouselAutoPlayHoverPause, model.CarouselAutoPlayHoverPause_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableCarouselLoop, model.EnableCarouselLoop_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableCarouselNavigation, model.EnableCarouselNavigation_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableCarouselPagination, model.EnableCarouselPagination_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CarouselPaginationClickable, model.CarouselPaginationClickable_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CarouselPaginationTypeId, model.CarouselPaginationTypeId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CarouselPaginationDynamicBullets, model.CarouselPaginationDynamicBullets_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.CarouselPaginationDynamicMainBullets, model.CarouselPaginationDynamicMainBullets_OverrideForStore, storeScope, false);

        #endregion

        #region Home page

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HideHomePageBestSellers, model.HideHomePageBestSellers_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HideHomePageFeaturedCategories, model.HideHomePageFeaturedCategories_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HideHomePageProducts, model.HideHomePageProducts_OverrideForStore, storeScope, false);

        #endregion

        #region Header

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableHeaderMenuOne, model.EnableHeaderMenuOne_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HeaderMenuOneTitle, model.HeaderMenuOneTitle_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HeaderMenuOneIconPictureId, model.HeaderMenuOneIconPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HeaderMenuOneLink, model.HeaderMenuOneLink_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableHeaderMenuTwo, model.EnableHeaderMenuTwo_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HeaderMenuTwoTitle, model.HeaderMenuTwoTitle_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HeaderMenuTwoIconPictureId, model.HeaderMenuTwoIconPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.HeaderMenuTwoLink, model.HeaderMenuTwoLink_OverrideForStore, storeScope, false);

        #endregion

        #region Footer description

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxOne, model.EnableFooterDescriptionBoxOne_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxOneTitle, model.FooterDescriptionBoxOneTitle_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxOneText, model.FooterDescriptionBoxOneText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxOneUrl, model.FooterDescriptionBoxOneUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxOnePictureId, model.FooterDescriptionBoxOnePictureId_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxTwo, model.EnableFooterDescriptionBoxTwo_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoTitle, model.FooterDescriptionBoxTwoTitle_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoText, model.FooterDescriptionBoxTwoText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoUrl, model.FooterDescriptionBoxTwoUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxTwoPictureId, model.FooterDescriptionBoxTwoPictureId_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxThree, model.EnableFooterDescriptionBoxThree_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeTitle, model.FooterDescriptionBoxThreeTitle_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeText, model.FooterDescriptionBoxThreeText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxThreeUrl, model.FooterDescriptionBoxThreeUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxThreePictureId, model.FooterDescriptionBoxThreePictureId_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.EnableFooterDescriptionBoxFour, model.EnableFooterDescriptionBoxFour_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxFourTitle, model.FooterDescriptionBoxFourTitle_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxFourText, model.FooterDescriptionBoxFourText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxFourUrl, model.FooterDescriptionBoxFourUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(kingdomSettings, x => x.FooterDescriptionBoxFourPictureId, model.FooterDescriptionBoxFourPictureId_OverrideForStore, storeScope, false);

        #endregion

        await _settingService.ClearCacheAsync();
        await UpdateCssFilesAsync(storeScope);

        //localization. no multi-store support for localization yet.
        foreach (var localized in model.Locales)
        {
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterContactUsText, localized.LanguageId, localized.FooterContactUsText);

            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.HeaderMenuOneTitle, localized.LanguageId, localized.HeaderMenuOneTitle);

            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.HeaderMenuTwoTitle, localized.LanguageId, localized.HeaderMenuTwoTitle);

            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxOneTitle, localized.LanguageId, localized.FooterDescriptionBoxOneTitle);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxOneText, localized.LanguageId, localized.FooterDescriptionBoxOneText);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxOneUrl, localized.LanguageId, localized.FooterDescriptionBoxOneUrl);

            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxTwoTitle, localized.LanguageId, localized.FooterDescriptionBoxTwoTitle);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxTwoText, localized.LanguageId, localized.FooterDescriptionBoxTwoText);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxTwoUrl, localized.LanguageId, localized.FooterDescriptionBoxTwoUrl);

            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxThreeTitle, localized.LanguageId, localized.FooterDescriptionBoxThreeTitle);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxThreeText, localized.LanguageId, localized.FooterDescriptionBoxThreeText);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxThreeUrl, localized.LanguageId, localized.FooterDescriptionBoxThreeUrl);

            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxFourTitle, localized.LanguageId, localized.FooterDescriptionBoxFourTitle);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxFourText, localized.LanguageId, localized.FooterDescriptionBoxFourText);
            await _localizationService.SaveLocalizedSettingAsync(kingdomSettings,
                x => x.FooterDescriptionBoxFourUrl, localized.LanguageId, localized.FooterDescriptionBoxFourUrl);
        }

        await _staticCacheManager.RemoveByPrefixAsync(ModelCacheKey.FooterDescriptionModelPattern);
        await _staticCacheManager.RemoveByPrefixAsync(ModelCacheKey.TopMenuModelPattern);
        await _staticCacheManager.RemoveByPrefixAsync(ModelCacheKey.SocialModelPattern);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));
        return RedirectToAction("Configure");
    }

    #endregion
}
