using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a recurring payment history entity builder
    /// </summary>
    public partial class RecurringPaymentHistoryBuilder : ZarayeEntityBuilder<RecurringPaymentHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(RecurringPaymentHistory.RecurringPaymentId)).AsInt32().ForeignKey<RecurringPayment>();
        }

        #endregion
    }
}