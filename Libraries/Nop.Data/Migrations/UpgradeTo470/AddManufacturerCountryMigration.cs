using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.UpgradeTo470;

[NopSchemaMigration("2024-12-01 00:00:00", "Add ManufacturerCountry column to Product")]
public class AddManufacturerCountryMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var productTableName = nameof(Product);
        var columnName = nameof(Product.ManufacturerCountry);

        if (!Schema.Table(productTableName).Column(columnName).Exists())
            Alter.Table(productTableName)
                .AddColumn(columnName).AsString(400).Nullable();
    }
}