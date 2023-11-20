using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a return request entity builder
    /// </summary>
    public partial class ReturnRequestBuilder : ZarayeEntityBuilder<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ReturnRequest.CustomerId)).AsInt32().ForeignKey<Customer>();
            table.WithColumn(nameof(ReturnRequest.OrderId)).AsInt32().ForeignKey<Order>();
            table.WithColumn(nameof(ReturnRequest.ReturnOrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}