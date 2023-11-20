using System.Data;
using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class OrderCalculationBuilder : ZarayeEntityBuilder<OrderCalculation>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderCalculation.OrderId)).AsInt32().NotNullable().ForeignKey<Order>(onDelete: Rule.Cascade);
        }
    }
}