using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipment entity builder
    /// </summary>
    public partial class ShipmentReturnReasonBuilder : ZarayeEntityBuilder<ShipmentReturnReason>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {

        }

        #endregion
    }
}