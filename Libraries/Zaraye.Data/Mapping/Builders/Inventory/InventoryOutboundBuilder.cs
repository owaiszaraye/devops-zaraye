using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Inventory
{
    public partial class InventoryOutboundBuilder : ZarayeEntityBuilder<InventoryOutbound>
    {
        #region Methods
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(InventoryOutbound.InventoryInboundId)).AsInt32().ForeignKey<InventoryInbound>();
            table.WithColumn(nameof(InventoryOutbound.SaleOrderId)).AsInt32().ForeignKey<Order>();
            table.WithColumn(nameof(InventoryOutbound.OrderItemId)).AsInt32().ForeignKey<OrderItem>();
            table.WithColumn(nameof(InventoryOutbound.ShipmentId)).AsInt32().ForeignKey<Shipment>();
        }

        #endregion
    }
}
