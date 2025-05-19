using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Themes.NopGadget.Models;
using NopStation.Plugin.Theme.NopGadget.Domain;

namespace Nop.Plugin.Themes.NopGadget.Data.Migrations
{
    [NopMigration("2025-05-19 01:00:00", "NopGadget Theme Plugin - Create ThemeSettings Table", MigrationProcessType.Plugin)]
    public class CreateNopGadgetThemeSettingsTable : Migration
    {
        public override void Up()
        {
            Create.TableFor<NopGadgetSettings>();
        }

        public override void Down()
        {
            Delete.Table(nameof(NopGadgetSettings));
        }
    }
}
