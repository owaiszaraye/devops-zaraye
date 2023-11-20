using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Shipping
{
    internal class ShipmentDropOffHistoryBuilder : ZarayeEntityBuilder<ShipmentDropOffHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ShipmentDropOffHistory.ShipmentId)).AsInt32().ForeignKey<Shipment>();
        }

        #endregion
    }
}
