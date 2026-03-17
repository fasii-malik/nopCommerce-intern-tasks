using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.WarrantySystem.Domain;

namespace Nop.Plugin.Misc.WarrantySystem.Migrations;

[NopMigration("2026/03/03 12:00:00", "WarrantySystem base schema")]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<WarrantyRegistration>();
    }
}