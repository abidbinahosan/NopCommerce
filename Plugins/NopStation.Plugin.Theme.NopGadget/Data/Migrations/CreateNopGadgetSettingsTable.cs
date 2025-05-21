using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using NopStation.Plugin.Theme.NopGadget.Domain;

namespace NopStation.Plugin.Theme.NopGadget.Data.Migrations
{
    [NopMigration("2025/05/20 03:12:49", "NopGadget theme settings table", MigrationProcessType.Installation)]
    public class CreateNopGadgetSettingsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<NopGadgetSettings>();
        }
    }
}
