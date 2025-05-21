using System.Linq;
using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using NopStation.Plugin.Misc.Core.Services;

namespace NopStation.Plugin.Theme.Kingdom.Migrations;

[NopMigration("2023-01-09 00:00:00", "4.50.2.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
public class DataMigration45020 : Migration
{
    private readonly ILocalizationService _localizationService;
    private readonly INopStationPluginManager _nopStationPluginManager;

    public DataMigration45020(ILocalizationService localizationService,
        INopStationPluginManager nopStationPluginManager)
    {
        _localizationService = localizationService;
        _nopStationPluginManager = nopStationPluginManager;
    }

    public override void Up()
    {
        //locales
        _localizationService.DeleteLocaleResourcesAsync("Admin.NopStation.Theme.Kingdom.").Wait();
        _localizationService.DeleteLocaleResourcesAsync("NopStation.Theme.Kingdom.").Wait();

        var plugin = _nopStationPluginManager.LoadNopStationPluginsAsync(pluginSystemName: "NopStation.Plugin.Theme.Kingdom").Result.FirstOrDefault();
        foreach (var keyValuePair in plugin.PluginResouces())
            _localizationService.AddOrUpdateLocaleResourceAsync(keyValuePair.Key, keyValuePair.Value).Wait();
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
