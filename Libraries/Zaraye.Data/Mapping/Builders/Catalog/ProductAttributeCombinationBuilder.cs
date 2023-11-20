﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product attribute combination entity builder
    /// </summary>
    public partial class ProductAttributeCombinationBuilder : ZarayeEntityBuilder<ProductAttributeCombination>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAttributeCombination.Sku)).AsString(400).Nullable()
                .WithColumn(nameof(ProductAttributeCombination.ManufacturerPartNumber)).AsString(400).Nullable()
                .WithColumn(nameof(ProductAttributeCombination.Gtin)).AsString(400).Nullable()
                .WithColumn(nameof(ProductAttributeCombination.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}