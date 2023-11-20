using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.CustomerLedgers
{
    public partial class CustomerLedgerBuilder:ZarayeEntityBuilder<CustomerLedger>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerLedger.Description)).AsString().Nullable()
                .WithColumn(nameof(CustomerLedger.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerLedger), nameof(CustomerLedger.PaymentId))).AsInt32().ForeignKey<Payment>(onDelete: Rule.None).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerLedger), nameof(CustomerLedger.ShipmentId))).AsInt32().ForeignKey<Shipment>(onDelete: Rule.None).Nullable();
        }

        #endregion
    }
}
