using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;
using System.Data;

namespace Zaraye.Data.Mapping.Builders.Orders
{


    public partial class OrderBuilder : ZarayeEntityBuilder<Order>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Order.CustomOrderNumber)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Order.BillingAddressId)).AsInt32().ForeignKey<Address>(onDelete: Rule.None)
                .WithColumn(nameof(Order.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(Order.PickupAddressId)).AsInt32().Nullable().ForeignKey<Address>(onDelete: Rule.None)
                .WithColumn(nameof(Order.ShippingAddressId)).AsInt32().Nullable().ForeignKey<Address>(onDelete: Rule.None)
                .WithColumn(nameof(Order.QuotationId)).AsInt32().ForeignKey<Quotation>(onDelete: Rule.None).Nullable()
;
        }

        #endregion
    }
}