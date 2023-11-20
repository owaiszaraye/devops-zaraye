using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    
    public partial class OrderDeliveryScheduleBuilder : ZarayeEntityBuilder<OrderDeliverySchedule>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(OrderDeliverySchedule.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}