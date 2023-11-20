using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a stock quantity history entity builder
    /// </summary>
    public partial class StockQuantityHistoryBuilder : ZarayeEntityBuilder<StockQuantityHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StockQuantityHistory.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}