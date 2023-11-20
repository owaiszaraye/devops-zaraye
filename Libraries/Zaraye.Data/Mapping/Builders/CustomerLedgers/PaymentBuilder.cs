using FluentMigrator.Builders.Create.Table;
using Zaraye.Data.Extensions;
using System.Data;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;

namespace Zaraye.Data.Mapping.Builders.CustomerLedgers
{
    public partial class PaymentBuilder : ZarayeEntityBuilder<Payment>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Payment.Amount)).AsDecimal().NotNullable()
                .WithColumn(nameof(Payment.CustomerId)).AsInt32().ForeignKey<Customer>().NotNullable()
                .WithColumn(nameof(Payment.BankDetailId)).AsInt32().ForeignKey<BankDetail>(onDelete: Rule.None).Nullable();
        }

        #endregion
    }
}
