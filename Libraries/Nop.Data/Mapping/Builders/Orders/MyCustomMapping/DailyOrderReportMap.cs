using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.Mapping.Builders.Orders.MyCustomMapping;
public class DailyOrderReportBuilder : NopEntityBuilder<DailyOrderReport>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DailyOrderReport.ReportDate)).AsDateTime2().NotNullable()
            .WithColumn(nameof(DailyOrderReport.TotalOrders)).AsInt32().NotNullable()
            .WithColumn(nameof(DailyOrderReport.TotalRevenue)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(DailyOrderReport.TopSellingProductId)).AsInt32().NotNullable()
            .WithColumn(nameof(DailyOrderReport.TopSellingProductName)).AsString(500).NotNullable()
            .WithColumn(nameof(DailyOrderReport.CreatedOnUtc)).AsDateTime2().NotNullable();
    }
}