using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Data.Migrations.MyCustomMigrations;

[NopMigration("2025-01-02 00:00:00", "Seed DailyOrderReportTask schedule")]
public class SeedDailyOrderReportTask : Migration
{
    // Hardcode the assembly-qualified name — no Services reference needed
    private const string TaskType =
    "Nop.Services.Orders.MyCustomTasks.DailyOrderReportTask, Nop.Services";

    public override void Up()
    {
        Execute.Sql($@"
            IF NOT EXISTS (
                SELECT 1 FROM [ScheduleTask] WHERE [Type] = '{TaskType}'
            )
            BEGIN
                INSERT INTO [ScheduleTask] ([Name], [Seconds], [Type], [Enabled], [StopOnError])
                VALUES ('Generate daily order report', 86400, '{TaskType}', 1, 0)
            END");
    }

    public override void Down()
    {
        Execute.Sql($"DELETE FROM [ScheduleTask] WHERE [Type] = '{TaskType}'");
    }
}