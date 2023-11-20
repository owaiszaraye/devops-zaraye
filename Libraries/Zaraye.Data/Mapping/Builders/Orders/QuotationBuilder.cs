using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Orders;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    public partial class QuotationBuilder : ZarayeEntityBuilder<Quotation>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Quotation.SupplierId)).AsInt32().NotNullable();
        }

        #endregion
    }
}