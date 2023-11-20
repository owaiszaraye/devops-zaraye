using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class TransporterVehicleMappingBuilder : ZarayeEntityBuilder<TransporterVehicleMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TransporterVehicleMapping.VehicleNumber)).AsString(400).NotNullable();
        }
    }
}