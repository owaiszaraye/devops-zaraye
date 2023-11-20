using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Inventory
{
    public partial class CogsInventoryTaggingBuilder : ZarayeEntityBuilder<CogsInventoryTagging>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CogsInventoryTagging.RequestId)).AsInt32().ForeignKey<Request>()
                .WithColumn(nameof(CogsInventoryTagging.InventoryId)).AsInt32().ForeignKey<InventoryInbound>();
        }
    }
}
