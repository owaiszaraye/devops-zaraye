using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    public partial class OrderCancellationBuilder : ZarayeEntityBuilder<OrderCancellation>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderCancellation.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}