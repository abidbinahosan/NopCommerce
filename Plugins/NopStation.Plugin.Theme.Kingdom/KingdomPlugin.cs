using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using NopStation.Plugin.Misc.Core;
using NopStation.Plugin.Misc.Core.Services;
using NopStation.Plugin.Theme.Kingdom.Components;

namespace NopStation.Plugin.Theme.Kingdom;

public class KingdomPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
{
    #region Fields

    public bool HideInWidgetList => false;

    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly IPictureService _pictureService;
    private readonly INopFileProvider _fileProvider;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly INopStationCoreService _nopStationCoreService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;
    private readonly IEmailAccountService _emailAccountService;

    #endregion

    #region Ctor

    public KingdomPlugin(ISettingService settingService,
        IWebHelper webHelper,
        INopFileProvider nopFileProvider,
        IPictureService pictureService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        INopStationCoreService nopStationCoreService,
        IStoreContext storeContext,
        IStoreService storeService,
        IEmailAccountService emailAccountService)
    {
        _settingService = settingService;
        _webHelper = webHelper;
        _fileProvider = nopFileProvider;
        _pictureService = pictureService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _nopStationCoreService = nopStationCoreService;
        _storeContext = storeContext;
        _storeService = storeService;
        _emailAccountService = emailAccountService;
    }

    #endregion

    #region Utilities

    private async Task CreateSampleDataAsync()
    {
        var sampleImagesPath = _fileProvider.MapPath("~/Plugins/NopStation.Plugin.Theme.Kingdom/Content/sample/");
        var store = await _storeContext.GetCurrentStoreAsync() ?? (await _storeService.GetAllStoresAsync()).FirstOrDefault();
        var email = (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

        var settings = new KingdomSettings()
        {
            EnableImageLazyLoad = true,
            LazyLoadPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "lazy-load.png")), MimeTypes.ImagePng, "lazy-load")).Id,
            FooterSupportedCardsPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-card-icons.png")), MimeTypes.ImagePng, "footer-cards")).Id,
            FooterLogoPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-logo-white.png")), MimeTypes.ImagePng, "footer-logo")).Id,
            ShowLogoAtFooter = true,
            ShowSupportedCardsAtFooter = true,
            FooterDescriptionBoxOneTitle = "Support 24/7",
            FooterDescriptionBoxOneText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
            FooterDescriptionBoxOnePictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "support-24-7.png")), MimeTypes.ImagePng, "support-24-7")).Id,
            FooterDescriptionBoxTwoTitle = "30 Days Return",
            FooterDescriptionBoxTwoText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
            FooterDescriptionBoxTwoPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "30-days-return-policy.png")), MimeTypes.ImagePng, "30-days-return-policy")).Id,
            FooterDescriptionBoxThreeTitle = "Global Shipping",
            FooterDescriptionBoxThreeText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
            FooterDescriptionBoxThreePictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "worldwide-shpping.png")), MimeTypes.ImagePng, "worldwide-shpping")).Id,
            FooterDescriptionBoxFourTitle = "Free Delivery",
            FooterDescriptionBoxFourText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
            FooterDescriptionBoxFourPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "free-delivery-icon.png")), MimeTypes.ImagePng, "free-delivery-icon")).Id,
            FooterContactUsText = $"<p>{store?.CompanyAddress}</p><div>&nbsp;</div><div style=\"line-height: 1.5;\">Email : {email?.Email}</div><div style=\"line-height: 1.5;\">Phone : +8801719304086</div>",
            CategoryEvenColor = KingdomDefaults.CategoryEvenThemeColor,
            CategoryOddColor = KingdomDefaults.CategoryOddThemeColor,
            HideDesignedByNopStationAtFooter = false,
            EnableFooterDescriptionBoxFour = true,
            EnableFooterDescriptionBoxOne = true,
            EnableFooterDescriptionBoxThree = true,
            EnableFooterDescriptionBoxTwo = true,
            EnableHeaderMenuOne = true,
            EnableHeaderMenuTwo = true,
            EnableStickyHeader = true,
            HideHomePageBestSellers = true,
            HideHomePageFeaturedCategories = true,
            HideHomePageProducts = true,
            HideProductBoxButtonsOnMobile = true,
            HideReviewInProductBox = true,
            SecondaryThemeColor = KingdomDefaults.SecondaryThemeColor,
            PrimaryThemeColor = KingdomDefaults.PrimaryThemeColor,
            NuberOfItemsToShowInProductFilters = 5,
            PhoneNumber = "+8801719304086",
            HeaderMenuOneLink = "/conditions-of-use",
            HeaderMenuTwoLink = "#",
            PinterestLink = "#",
            InstagramLink = "#",
            HeaderMenuOneTitle = "Terms & Conditions",
            CustomCss = "body {" + Environment.NewLine + "}",
            HeaderMenuOneIconPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "header-menu1.png")), MimeTypes.ImagePng, "header-menu1")).Id,
            HeaderMenuTwoIconPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "header-menu2.png")), MimeTypes.ImagePng, "header-menu2")).Id,
            HeaderMenuTwoTitle = "Store Location",
            CarouselAutoPlayHoverPause = true,
            CarouselAutoPlayTimeout = 3000,
            CarouselPaginationClickable = true,
            CarouselPaginationDynamicBullets = true,
            CarouselPaginationDynamicMainBullets = 3,
            CarouselPaginationTypeId = (int)PaginationType.Bullets,
            EnableCarouselAutoPlay = true,
            EnableCarouselLoop = true,
            EnableCarouselNavigation = true,
            EnableCarouselPagination = true,
            EnableCarouselsForDefaultEntityLists = true,
            ProductDetailsPageSupportedCardsPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-card-icons.png")), MimeTypes.ImagePng, "footer-cards")).Id,
            ShowSupportedCardsInProductDetailsPage = true,
            EnableLoginBoxAtHeader = true,
            FooterDescriptionBoxFourUrl = "",
            FooterDescriptionBoxOneUrl = "",
            FooterDescriptionBoxThreeUrl = "",
            FooterDescriptionBoxTwoUrl = ""
        };
        await _settingService.SaveSettingAsync(settings);
    }

    #endregion

    #region Methods

    public override string GetConfigurationPageUrl()
    {
        return _webHelper.GetStoreLocation() + "Admin/Kingdom/Configure";
    }

    public async override Task InstallAsync()
    {
        await CreateSampleDataAsync();
        await this.InstallPluginAsync(new KingdomPermissionProvider());
        await base.InstallAsync();
    }

    public async override Task UninstallAsync()
    {
        await this.UninstallPluginAsync(new KingdomPermissionProvider());
        await base.UninstallAsync();
    }

    public async Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        var menuItem = new SiteMapNode()
        {
            Title = await _localizationService.GetResourceAsync("Admin.NopStation.Theme.Kingdom.Menu.Kingdom"),
            Visible = true,
            IconClass = "far fa-dot-circle",
        };

        if (await _permissionService.AuthorizeAsync(KingdomPermissionProvider.ManageKingdom))
        {
            var configItem = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.Theme.Kingdom.Menu.Configuration"),
                Url = "~/Admin/Kingdom/Configure",
                Visible = true,
                IconClass = "far fa-circle",
                SystemName = "Kingdom.Configuration"
            };
            menuItem.ChildNodes.Add(configItem);
        }

        if (await _permissionService.AuthorizeAsync(CorePermissionProvider.ShowDocumentations))
        {
            var documentation = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.Common.Menu.Documentation"),
                Url = "https://www.nop-station.com/kingdom-theme-documentation?utm_source=admin-panel&utm_medium=products&utm_campaign=kingdom-theme",
                Visible = true,
                IconClass = "far fa-circle",
                OpenUrlInNewTab = true
            };
            menuItem.ChildNodes.Add(documentation);
        }

        await _nopStationCoreService.ManageSiteMapAsync(rootNode, menuItem, NopStationMenuType.Theme);
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == KingdomDefaults.FooterBeforeWidgetZone)
            return typeof(KingdomFooterViewComponent);

        if (widgetZone == KingdomDefaults.HeaderSelectorsWidgetZone)
            return typeof(KingdomHeaderLinkViewComponent);

        if (widgetZone == KingdomDefaults.HeaderLinksMiddleWidgetZone)
            return typeof(KingdomHeaderLoginViewComponent);

        if (widgetZone == KingdomDefaults.SocialButtonsAfterWidgetZone)
            return typeof(KingdomSocialButtonsViewComponent);

        return typeof(KingdomViewComponent);
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        var result = new List<string> {
            KingdomDefaults.HeaderLinksMiddleWidgetZone,
            KingdomDefaults.HeaderSelectorsWidgetZone,
            KingdomDefaults.FooterBeforeWidgetZone,
            KingdomDefaults.SocialButtonsAfterWidgetZone,
            PublicWidgetZones.Footer
        };

        return Task.FromResult<IList<string>>(result);
    }

    public List<KeyValuePair<string, string>> PluginResouces()
    {
        var list = new Dictionary<string, string>()
        {
            ["Admin.NopStation.Theme.Kingdom.Menu.Kingdom"] = "Kingdom",
            ["Admin.NopStation.Theme.Kingdom.Menu.Configuration"] = "Configuration",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CategoryEvenColor"] = "Category even color",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CategoryEvenColor.Hint"] = "Choose a color for even category.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CategoryOddColor"] = "Category odd color",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CategoryOddColor.Hint"] = "Choose a color for odd category.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.PrimaryThemeColor"] = "Primary theme color",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.PrimaryThemeColor.Hint"] = "Choose primary color for theme.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.SecondaryThemeColor"] = "Secondary theme color",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.SecondaryThemeColor.Hint"] = "Choose secondary color for theme.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.PhoneNumber"] = "Phone number",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.PhoneNumber.Hint"] = "Specify phone number which which will be displayed in public store.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.InstagramLink"] = "Instagram Link",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.InstagramLink.Hint"] = "Specify your Instagram link which will be displayed in your public store.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.PinterestLink"] = "Pinterest Link",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.PinterestLink.Hint"] = "Specify your Pinterest link which will be displayed in your public store.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableLoginBoxAtHeader"] = "Enable login box at header",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableLoginBoxAtHeader.Hint"] = "Check to enable login box at header.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableImageLazyLoad"] = "Enable image lazy-load",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableImageLazyLoad.Hint"] = "Check to enable lazy-load for product box image.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.LazyLoadPictureId"] = "Lazy-load picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.LazyLoadPictureId.Hint"] = "This picture will be displayed initially in product box. Uploaded picture size should not be more than 4-5 KB.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideReviewInProductBox"] = "Hide review in product box",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideReviewInProductBox.Hint"] = "Check to hide review in product box.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableStickyHeader"] = "Enable stickey header",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableStickyHeader.Hint"] = "Check to enable stickey header.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideProductBoxButtonsOnMobile"] = "Hide product box buttons on mobile",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideProductBoxButtonsOnMobile.Hint"] = "Check to hide product box buttons on mobile.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.NuberOfItemsToShowInProductFilters"] = "Nuber of items to show in product filters",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.NuberOfItemsToShowInProductFilters.Hint"] = "Define nuber of items to show in product filters.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CustomCss"] = "Custom CSS",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CustomCss.Hint"] = "Write custom CSS for your site.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterContactUsText"] = "Footer. Contact us text",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterContactUsText.Hint"] = "Footer. Contact us text at footer.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideDesignedByNopStationAtFooter"] = "Footer. Hide \"designed by nopStation\"",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideDesignedByNopStationAtFooter.Hint"] = "Footer. Check to hide \"designed by nopStation\" at footer.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ShowLogoAtFooter"] = "Footer. Show logo",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ShowLogoAtFooter.Hint"] = "Footer. Check to show logo at footer.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterLogoPictureId"] = "Footer. Logo",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterLogoPictureId.Hint"] = "Footer. Logo to display at footer.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ShowSupportedCardsAtFooter"] = "Footer. Show supported cards",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ShowSupportedCardsAtFooter.Hint"] = "Footer. Check to show supported cards at footer.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterSupportedCardsPictureId"] = "Footer. Supported cards picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterSupportedCardsPictureId.Hint"] = "Footer. Select footer supported cards picture.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ShowSupportedCardsInProductDetailsPage"] = "Product details page. Show supported cards",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ShowSupportedCardsInProductDetailsPage.Hint"] = "Product details page. Check to show supported cards in product details page.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ProductDetailsPageSupportedCardsPictureId"] = "Product details page. Supported cards picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.ProductDetailsPageSupportedCardsPictureId.Hint"] = "Product details page. Select product details page supported cards picture.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselsForDefaultEntityLists"] = "Enable carousels for default entity lists",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselsForDefaultEntityLists.Hint"] = "Check to enable carousels for default entity lists (i.e. Home page categories, Featured products, Related products, Products also purchased, Cross sell products).",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselAutoPlay"] = "Carousel. Enable auto play",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselAutoPlay.Hint"] = "Carousel. Check to enable auto play.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselAutoPlayTimeout"] = "Carousel. Auto play timeout",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselAutoPlayTimeout.Hint"] = "Carousel. It's autoplay interval timeout. (e.g 5000)",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselAutoPlayHoverPause"] = "Carousel. Auto play hover pause",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselAutoPlayHoverPause.Hint"] = "Carousel. Check to enable pause on mouse hover.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselLoop"] = "Carousel. Enable loop",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselLoop.Hint"] = "Carousel. Check to enable loop.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselNavigation"] = "Carousel. Enable navigation",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselNavigation.Hint"] = "Carousel. Check to enable next/prev buttons.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselPagination"] = "Carousel. Enable pagination",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableCarouselPagination.Hint"] = "Carousel. Check to enable pagination.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationTypeId"] = "Carousel. Pagination type",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationTypeId.Hint"] = "Carousel. Select pagination type.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationDynamicMainBullets"] = "Carousel. Dynamic main bullets",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationDynamicMainBullets.Hint"] = "Carousel. The number of main bullets visible when dynamic bullets enabled.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationDynamicBullets"] = "Carousel. Dynamic bullets",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationDynamicBullets.Hint"] = "Carousel. Good to enable if you use bullets pagination with a lot of slides. So it will keep only few bullets visible at the same time.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationClickable"] = "Carousel. Pagination clickable",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.CarouselPaginationClickable.Hint"] = "Carousel. If true then clicking on pagination button will cause transition to appropriate slide. Only for bullets pagination type.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideHomePageBestSellers"] = "Home page. Hide best sellers",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideHomePageBestSellers.Hint"] = "Home page. Check to hide home page best sellers.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideHomePageFeaturedCategories"] = "Home page. Hide featured categories",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideHomePageFeaturedCategories.Hint"] = "Home page. Check to hide home page featured categories.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideHomePageProducts"] = "Home page. Hide home page products",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HideHomePageProducts.Hint"] = "Home page. Check to hide home page products.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableHeaderMenuOne"] = "Header menu. Enable menu one",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableHeaderMenuOne.Hint"] = "Header menu. Check to enable header menu one.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuOneTitle"] = "Header menu. Menu one title",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuOneTitle.Hint"] = "Header menu. Enter header menu one title.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuOneIconPictureId"] = "Header menu. Menu one icon",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuOneIconPictureId.Hint"] = "Header menu. Select header menu one icon.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuOneLink"] = "Header menu. Menu one link",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuOneLink.Hint"] = "Header menu. Enter header menu one link.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableHeaderMenuTwo"] = "Header menu. Enable menu two",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableHeaderMenuTwo.Hint"] = "Header menu. Check to enable header menu two.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuTwoTitle"] = "Header menu. Menu two title",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuTwoTitle.Hint"] = "Header menu. Enter header menu two title.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuTwoIconPictureId"] = "Header menu. Menu two icon",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuTwoIconPictureId.Hint"] = "Header menu. Select header menu two icon.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuTwoLink"] = "Header menu. Menu two link",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.HeaderMenuTwoLink.Hint"] = "Header menu. Enter header menu two link.",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxOne"] = "Description box. Enable box one",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxOne.Hint"] = "Description box. Check to enable description box one.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOneTitle"] = "Description box. Box one title",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOneTitle.Hint"] = "Description box. Enter box one title.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOneText"] = "Description box. Box one text",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOneText.Hint"] = "Description box. Enter description box one text.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOneUrl"] = "Description box. Box one URL",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOneUrl.Hint"] = "Description box. Enter description box one URL.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOnePictureId"] = "Description box. Box one picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxOnePictureId.Hint"] = "Description box. Enter description box one picture.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxTwo"] = "Description box. Enable box two",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxTwo.Hint"] = "Description box. Check to enable description box two.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoTitle"] = "Description box. Box two title",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoTitle.Hint"] = "Description box. Enter description box two title.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoText"] = "Description box. Box two text",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoText.Hint"] = "Description box. Enter description box two text.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoUrl"] = "Description box. Box two URL",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoUrl.Hint"] = "Description box. Enter description box two URL.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoPictureId"] = "Description box. Box two picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxTwoPictureId.Hint"] = "Description box. Enter description box two picture.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxThree"] = "Description box. Enable box three",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxThree.Hint"] = "Description box. Check to enable description box three.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreeTitle"] = "Description box. Box three title",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreeTitle.Hint"] = "Description box. Enter description box three title.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreeText"] = "Description box. Box three text",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreeText.Hint"] = "Description box. Enter description box three text.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreeUrl"] = "Description box. Box three URL",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreeUrl.Hint"] = "Description box. Enter description box three URL.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreePictureId"] = "Description box. Box three picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxThreePictureId.Hint"] = "Description box. Enter description box three picture.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxFour"] = "Description box. Enable box four",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.EnableFooterDescriptionBoxFour.Hint"] = "Description box. Check to enable description box four.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourTitle"] = "Description box. Box four title",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourTitle.Hint"] = "Description box. Enter description box four title.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourText"] = "Description box. Box four text",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourText.Hint"] = "Description box. Enter description box four text.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourUrl"] = "Description box. Box four URL",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourUrl.Hint"] = "Description box. Enter description box four URL.",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourPictureId"] = "Description box. Box four picture",
            ["Admin.NopStation.Theme.Kingdom.Configuration.Fields.FooterDescriptionBoxFourPictureId.Hint"] = "Description box. Enter description box four picture.",

            ["Admin.NopStation.Theme.Kingdom.Configuration"] = "Kingdom theme settings",
            ["Admin.NopStation.Theme.Kingdom.Configuration.TabTitle.Color"] = "Color",
            ["Admin.NopStation.Theme.Kingdom.Configuration.TabTitle.General"] = "General",
            ["Admin.NopStation.Theme.Kingdom.Configuration.TabTitle.Carousel"] = "Carousel",
            ["Admin.NopStation.Theme.Kingdom.Configuration.TabTitle.HomePage"] = "Home page",
            ["Admin.NopStation.Theme.Kingdom.Configuration.TabTitle.HeaderTop"] = "Header top",
            ["Admin.NopStation.Theme.Kingdom.Configuration.TabTitle.FooterDescription"] = "Footer description",

            ["Admin.NopStation.Theme.Kingdom.Configuration.FooterDescriptionOne"] = "Description one",
            ["Admin.NopStation.Theme.Kingdom.Configuration.FooterDescriptionTwo"] = "Description two",
            ["Admin.NopStation.Theme.Kingdom.Configuration.FooterDescriptionThree"] = "Description three",
            ["Admin.NopStation.Theme.Kingdom.Configuration.FooterDescriptionFour"] = "Description four",
            ["Admin.NopStation.Theme.Kingdom.Configuration.HeaderMenuOne"] = "Menu one",
            ["Admin.NopStation.Theme.Kingdom.Configuration.HeaderMenuTwo"] = "Menu two",

            ["Admin.NopStation.Theme.Kingdom.Configuration.Carousel.Hint"] = "Settings to configure carousel for Home page categories (home page), Featured products (home page), Related products (product details page), Products also purchased (product details page), Cross sell products (cart page)",

            ["NopStation.Theme.Kingdom.Picture.AlterText"] = "Picture of {0}",
            ["NopStation.Theme.Kingdom.Picture.Title"] = "Picture of {0}",
            ["NopStation.Theme.Kingdom.FooterSupportedCards.AlterText"] = "Supported cards",
            ["NopStation.Theme.Kingdom.FooterSupportedCards.Title"] = "Supported cards",
            ["NopStation.Theme.Kingdom.ProductDetailsSupportedCards.AlterText"] = "Supported cards",
            ["NopStation.Theme.Kingdom.ProductDetailsSupportedCards.Title"] = "Supported cards",
            ["NopStation.Theme.Kingdom.ProductDetailsSupportedCards.WeAccept"] = "Supported cards",
            ["NopStation.Theme.Kingdom.FooterLogo.AlterText"] = "Logo of {0}",
            ["NopStation.Theme.Kingdom.FooterLogo.Title"] = "Logo of {0}",

            ["NopStation.Theme.Kingdom.UserAccount"] = "Account",
            ["NopStation.Theme.Kingdom.Footer.ContactUs"] = "Contact Us",
            ["NopStation.Theme.Kingdom.HeaderLogin.DoNotHaveAccount"] = "Don't have an account?",
            ["NopStation.Theme.Kingdom.HeaderLogin.CreateAccount"] = "Create an account",
            ["NopStation.Theme.Kingdom.HomePage.FeaturedCategories"] = "Featured Categories",
            ["NopStation.Theme.Kingdom.HomePage.BlogNews.LatestNews"] = "Latest News",
            ["NopStation.Theme.Kingdom.HomePage.BlogNews.LatestBlogs"] = "Latest Blogs",
            ["NopStation.Theme.Kingdom.HomePage.BlogNews.ViewAll"] = "View All",
            ["NopStation.Theme.Kingdom.Catalog.PerPage"] = "Per Page",
            ["NopStation.Theme.Kingdom.ProductDetails.AddToWisthlist"] = "Wisthlist",
            ["NopStation.Theme.Kingdom.ProductDetails.AddToCompare"] = "Compare",
            ["NopStation.Theme.Kingdom.ProductDetails.FullDescription"] = "Full Description",
            ["NopStation.Theme.Kingdom.ProductDetails.SpecificationAttributes"] = "Specifications",
            ["NopStation.Theme.Kingdom.ProductDetails.Tags"] = "Tags",
            ["NopStation.Theme.Kingdom.ProductDetails.Reviews"] = "Product Reviews",
            ["NopStation.Theme.Kingdom.Mobile.ShoppingCart"] = "Cart",
            ["NopStation.Theme.Kingdom.Filter.Button"] = "Filter",
            ["Nopstation.Theme.Kingdom.order.complete"] = "Order Completed",
            ["NopStation.Theme.Kingdom.Filter.Min"] = "Min",
            ["NopStation.Theme.Kingdom.Filter.Max"] = "Max",
            ["common.wait..."] = "Wait",
            ["nopstation.theme.kingdom.carttable.info"] = "Cart Info",
            ["NopStation.Theme.Kingdom.Mobile.ShoppingCart"] = "Cart",
            ["NopStation.Theme.Kingdom.Footer.FollowUs.Instagram"] = "Instagram",
            ["NopStation.Theme.Kingdom.Footer.FollowUs.Pinterest"] = "Pinterest",
            ["NopStation.Theme.Kingdom.ProductDetails.Reviews"] = "Reviews",
        };

        return list.ToList();
    }

    #endregion
}