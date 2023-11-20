using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product warehouse inventory entity builder
    /// </summary>
    public partial class ProductWarehouseInventoryBuilder : ZarayeEntityBuilder<ProductWarehouseInventory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductWarehouseInventory.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductWarehouseInventory.WarehouseId)).AsInt32().ForeignKey<Warehouse>();
        }

        #endregion
    }
}