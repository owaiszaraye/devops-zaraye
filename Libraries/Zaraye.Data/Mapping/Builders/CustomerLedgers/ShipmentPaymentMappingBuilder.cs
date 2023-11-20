using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Data.Mapping.Builders.CustomerLedgers
{
    public partial class ShipmentPaymentMappingBuilder : ZarayeEntityBuilder<ShipmentPaymentMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShipmentPaymentMapping.ShipmentId)).AsInt32().ForeignKey<Shipment>().NotNullable()
                .WithColumn(nameof(ShipmentPaymentMapping.PaymentId)).AsInt32().ForeignKey<Payment>().NotNullable()
                .WithColumn(nameof(ShipmentPaymentMapping.Amount)).AsDecimal().NotNullable();

        }

        #endregion
    }
}
