﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order item entity builder
    /// </summary>
    public partial class OrderSalesReturnRequestBuilder : ZarayeEntityBuilder<OrderSalesReturnRequest>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(OrderSalesReturnRequest.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}