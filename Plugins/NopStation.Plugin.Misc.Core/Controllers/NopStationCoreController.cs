using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Security;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using NopStation.Plugin.Misc.Core.Filters;
using NopStation.Plugin.Misc.Core.Models;
using NopStation.Plugin.Misc.Core.Services;

namespace NopStation.Plugin.Misc.Core.Controllers;

public class NopStationCoreController : NopStationAdminController
{
    private readonly IStoreContext _storeContext;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IWorkContext _workContext;
    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly INopStationPluginManager _nopStationPluginManager;
    private readonly ICustomerService _customerService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly INopFileProvider _fileProvider;

    public NopStationCoreController(IStoreContext storeContext,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IWorkContext workContext,
        IBaseAdminModelFactory baseAdminModelFactory,
        INopStationPluginManager nopStationPluginManager,
        ICustomerService customerService,
        INotificationService notificationService,
        ISettingService settingService,
        INopFileProvider fileProvider)
    {
        _storeContext = storeContext;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _workContext = workContext;
        _baseAdminModelFactory = baseAdminModelFactory;
        _nopStationPluginManager = nopStationPluginManager;
        _customerService = customerService;
        _notificationService = notificationService;
        _settingService = settingService;
        _fileProvider = fileProvider;
    }

    [CheckPermission(CorePermissionProvider.MANAGE_CONFIGURATION)]
    public async Task<IActionResult> Configure()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<NopStationCoreSettings>(storeScope);

        var model = new ConfigurationModel()
        {
            AllowedCustomerRoleIds = settings.AllowedCustomerRoleIds,
            RestrictMainMenuByCustomerRoles = settings.RestrictMainMenuByCustomerRoles,
        };
        model.ActiveStoreScopeConfiguration = storeScope;

        await _baseAdminModelFactory.PrepareCustomerRolesAsync(model.AvailableCustomerRoles, false);

        if (storeScope == 0)
            return View(model);

        model.AllowedCustomerRoleIds_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.AllowedCustomerRoleIds, storeScope);
        model.RestrictMainMenuByCustomerRoles_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.RestrictMainMenuByCustomerRoles, storeScope);

        return View("~/Plugins/NopStation.Core/Views/NopStationCore/Configure.cshtml", model);
    }

    [EditAccess, HttpPost]
    [CheckPermission(CorePermissionProvider.MANAGE_CONFIGURATION)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<NopStationCoreSettings>(storeScope);

        var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
        if (adminRole != null && !model.AllowedCustomerRoleIds.Contains(adminRole.Id))
        {
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.NopStation.Core.Configuration.AdminCanNotBeRestricted"));
            model.AllowedCustomerRoleIds.Add(adminRole.Id);
        }

        settings.AllowedCustomerRoleIds = model.AllowedCustomerRoleIds.ToList();
        settings.RestrictMainMenuByCustomerRoles = model.RestrictMainMenuByCustomerRoles;

        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.AllowedCustomerRoleIds, model.AllowedCustomerRoleIds_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.RestrictMainMenuByCustomerRoles, model.RestrictMainMenuByCustomerRoles_OverrideForStore, storeScope, false);

        await _settingService.ClearCacheAsync();
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

        return RedirectToAction("Configure");
    }

    [CheckPermission(CorePermissionProvider.MANAGE_CONFIGURATION)]
    public async Task<IActionResult> LocaleResource()
    {
        var searchModel = new CoreLocaleResourceSearchModel();
        searchModel.SearchLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        await _baseAdminModelFactory.PrepareLanguagesAsync(searchModel.AvailableLanguages, false);

        var plugins = await _nopStationPluginManager.LoadNopStationPluginsAsync(storeId: _storeContext.GetCurrentStoreAsync().Id);
        foreach (var item in plugins)
        {
            searchModel.AvailablePlugins.Add(new SelectListItem()
            {
                Value = item.PluginDescriptor.SystemName,
                Text = item.PluginDescriptor.FriendlyName
            });
        }
        searchModel.AvailablePlugins.Insert(0, new SelectListItem()
        {
            Value = "",
            Text = _localizationService.GetResourceAsync("Admin.NopStation.Core.Resources.List.SearchPluginSystemName.All").Result
        });

        return View(searchModel);
    }

    [HttpPost]
    [CheckPermission(CorePermissionProvider.MANAGE_CONFIGURATION)]
    public async Task<IActionResult> LocaleResource(CoreLocaleResourceSearchModel searchModel)
    {
        var resources = await _nopStationPluginManager.LoadPluginStringResourcesAsync(searchModel.SearchPluginSystemName,
            searchModel.SearchResourceName, searchModel.SearchLanguageId, _storeContext.GetCurrentStoreAsync().Id,
            searchModel.Page - 1, searchModel.PageSize);

        var model = new CoreLocaleResourceListModel().PrepareToGrid(searchModel, resources, () =>
        {
            return resources.Select(resource =>
            {
                return new CoreLocaleResourceModel()
                {
                    ResourceName = resource.Key.ToLower(),
                    ResourceValue = resource.Value,
                    ResourceNameLanguageId = $"{resource.Key}___{searchModel.SearchLanguageId}"
                };
            });
        });

        return Json(model);
    }

    [EditAccessAjax, HttpPost]
    [CheckPermission(CorePermissionProvider.MANAGE_CONFIGURATION)]
    public async Task<JsonResult> ResourceUpdate(CoreLocaleResourceModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ResourceNameLanguageId))
            return ErrorJson(_localizationService.GetResourceAsync("Admin.NopStation.Core.Resources.FailedToSave"));

        var token = model.ResourceNameLanguageId.Split(new[] { "___" }, StringSplitOptions.None);
        model.ResourceName = token[0];
        model.LanguageId = int.Parse(token[1]);

        if (model.ResourceValue != null)
            model.ResourceValue = model.ResourceValue.Trim();

        var resource = _localizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId).Result;

        if (resource != null)
        {
            resource.ResourceValue = model.ResourceValue;
            await _localizationService.UpdateLocaleStringResourceAsync(resource);
        }
        else
        {
            var rs = model.ToEntity<LocaleStringResource>();
            rs.LanguageId = model.LanguageId;
            await _localizationService.InsertLocaleStringResourceAsync(rs);
        }

        return new NullJsonResult();
    }

    [CheckPermission(CorePermissionProvider.MANAGE_LICENSE)]
    public async Task<IActionResult> AssemblyInfo()
    {
        var plugins = await _nopStationPluginManager.LoadNopStationPluginsAsync(storeId: _storeContext.GetCurrentStoreAsync().Id);

        var model = new List<PluginInfoModel>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("NopStation.Plugin") && !x.GetName().Name.EndsWith(".Views")).ToList();
        foreach (var assembly in assemblies)
        {
            var assemblyName = assembly.GetName();
            var filePath = assembly.IsDynamic ? null : assembly.Location;
            var attributes = assembly.GetCustomAttributes(false);
            var productAttribute = attributes
                .FirstOrDefault(x => x.GetType() == typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
            var descriptionAttribute = attributes
                .FirstOrDefault(x => x.GetType() == typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
            var buildType = "";
            if (attributes.FirstOrDefault(x => x.GetType() == typeof(DebuggableAttribute)) is DebuggableAttribute debAttr)
                buildType = debAttr.IsJITOptimizerDisabled ? "Debug" : "Release";
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);

            var mm = new PluginInfoModel()
            {
                FileName = _fileProvider.GetFileName(filePath),
                FilePath = filePath,
                AssemblyVersion = assemblyName.Version == null ? "" : assemblyName.Version.ToString(),
                AssemblyName = productAttribute?.Product,
                CreatedOn = _fileProvider.GetCreationTime(filePath),
                BuildType = buildType,
                FileVersion = fileVersionInfo?.FileVersion,
                Description = descriptionAttribute?.Description
            };
            model.Add(mm);
        }
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Permissions()
    {
        var model = new PermissionConfigurationModel();

        var customerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        model.AreCustomerRolesAvailable = customerRoles.Any();

        var permissionRecords = (await _permissionService.GetAllPermissionRecordsAsync()).Where(x => x.Category == "NopStation").ToList();
        model.IsPermissionsAvailable = permissionRecords.Any();

        return View(model);
    }
}