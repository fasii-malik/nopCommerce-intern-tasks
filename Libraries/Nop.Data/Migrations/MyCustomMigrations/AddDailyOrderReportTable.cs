using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Nop.Data.Migrations.MyCustomMigrations;

[NopSchemaMigration("2025-01-01 00:00:00", "Add DailyOrderReport table")]
public class AddDailyOrderReportTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<DailyOrderReport>();
    }
}