using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order note entity builder
    /// </summary>
    public partial class OrderNoteBuilder : ZarayeEntityBuilder<OrderNote>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderNote.Note)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(OrderNote.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}