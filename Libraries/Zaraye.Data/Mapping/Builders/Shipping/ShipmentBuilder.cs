using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;
using System.Data;

namespace Zaraye.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipment entity builder
    /// </summary>
    public partial class ShipmentBuilder : ZarayeEntityBuilder<Shipment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.DeliveryRequestId))).AsInt32().ForeignKey<OrderDeliveryRequest>(onDelete: Rule.None).Nullable()
                .WithColumn(nameof(Shipment.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}