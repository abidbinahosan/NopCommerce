using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Factories;

namespace NopStation.Plugin.Theme.Kingdom.Controllers;

public class KingdomController : BasePublicController
{
    #region Properties

    private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    private readonly IProductService _productService;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IPictureService _pictureService;
    private readonly ILocalizationService _localizationService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly ITaxService _taxService;
    private readonly ICurrencyService _currencyService;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    private readonly IDiscountService _discountService;
    private readonly ICustomerService _customerService;
    private readonly IGiftCardService _giftCardService;
    private readonly IDateRangeService _dateRangeService;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly IPermissionService _permissionService;
    private readonly IDownloadService _downloadService;
    private readonly IStaticCacheManager _cacheManager;
    private readonly IWebHelper _webHelper;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IVendorService _vendorService;

    private readonly MediaSettings _mediaSettings;
    private readonly ShoppingCartSettings _shoppingCartSettings;
    private readonly OrderSettings _orderSettings;
    private readonly CaptchaSettings _captchaSettings;
    private readonly CustomerSettings _customerSettings;
    private readonly IUserAgentHelper _userAgentHelper;

    #endregion

    #region Ctor

    public KingdomController(IShoppingCartModelFactory shoppingCartModelFactory,
        IProductService productService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IShoppingCartService shoppingCartService,
        IPictureService pictureService,
        ILocalizationService localizationService,
        IProductAttributeService productAttributeService,
        IProductAttributeParser productAttributeParser,
        ITaxService taxService, ICurrencyService currencyService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IDiscountService discountService,
        ICustomerService customerService,
        IGiftCardService giftCardService,
        IDateRangeService dateRangeService,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        IWorkflowMessageService workflowMessageService,
        IPermissionService permissionService,
        IDownloadService downloadService,
        IStaticCacheManager cacheManager,
        IWebHelper webHelper,
        ICustomerActivityService customerActivityService,
        IGenericAttributeService genericAttributeService,
        MediaSettings mediaSettings,
        ShoppingCartSettings shoppingCartSettings,
        OrderSettings orderSettings,
        CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        IProductModelFactory productModelFactory,
        IVendorService vendorService,
        IUserAgentHelper userAgentHelper
        )
    {
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _productService = productService;
        _workContext = workContext;
        _storeContext = storeContext;
        _shoppingCartService = shoppingCartService;
        _pictureService = pictureService;
        _localizationService = localizationService;
        _productAttributeService = productAttributeService;
        _productAttributeParser = productAttributeParser;
        _taxService = taxService;
        _currencyService = currencyService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _checkoutAttributeParser = checkoutAttributeParser;
        _discountService = discountService;
        _customerService = customerService;
        _giftCardService = giftCardService;
        _dateRangeService = dateRangeService;
        _checkoutAttributeService = checkoutAttributeService;
        _workflowMessageService = workflowMessageService;
        _permissionService = permissionService;
        _downloadService = downloadService;
        _cacheManager = cacheManager;
        _webHelper = webHelper;
        _customerActivityService = customerActivityService;
        _genericAttributeService = genericAttributeService;
        _productModelFactory = productModelFactory;

        _mediaSettings = mediaSettings;
        _shoppingCartSettings = shoppingCartSettings;
        _orderSettings = orderSettings;
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;

        _vendorService = vendorService;
        _userAgentHelper = userAgentHelper;
    }

    #endregion
}
