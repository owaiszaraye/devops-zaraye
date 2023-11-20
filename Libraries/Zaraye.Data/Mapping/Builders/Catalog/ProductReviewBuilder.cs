using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Stores;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product review entity builder
    /// </summary>
    public partial class ProductReviewBuilder : ZarayeEntityBuilder<ProductReview>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReview.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(ProductReview.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductReview.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}