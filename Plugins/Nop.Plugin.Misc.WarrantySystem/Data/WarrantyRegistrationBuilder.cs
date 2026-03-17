using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.WarrantySystem.Domain;

namespace Nop.Plugin.Misc.WarrantySystem.Data;

public class WarrantyRegistrationBuilder : NopEntityBuilder<WarrantyRegistration>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(WarrantyRegistration.CustomerId)).AsInt32().NotNullable()
            .WithColumn(nameof(WarrantyRegistration.ProductName)).AsString(400).NotNullable()
            .WithColumn(nameof(WarrantyRegistration.SerialNumber)).AsString(400).NotNullable()
            .WithColumn(nameof(WarrantyRegistration.InvoicePictureId)).AsInt32().Nullable()
            .WithColumn(nameof(WarrantyRegistration.Status)).AsInt32().NotNullable()
            .WithColumn(nameof(WarrantyRegistration.CreatedOnUtc)).AsDateTime2().NotNullable();
    }
}