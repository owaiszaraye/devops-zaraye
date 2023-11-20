using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order item entity builder
    /// </summary>
    public partial class ContractBuilder : ZarayeEntityBuilder<Contract>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Contract.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}