using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Shipping;

namespace Zaraye.Data.Mapping.Builders.Shipping
{
    public partial class DeliveryTimeReasonBuilder : ZarayeEntityBuilder<DeliveryTimeReason>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}