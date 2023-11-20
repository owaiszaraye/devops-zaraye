using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a order item entity builder
    /// </summary>
    public partial class RequestBuilder : ZarayeEntityBuilder<Request>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Request.CategoryId)).AsInt32().ForeignKey<Category>()
                .WithColumn(nameof(Request.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(Request.IndustryId)).AsInt32().ForeignKey<Industry>()
                .WithColumn(nameof(Request.BrandId)).AsInt32().ForeignKey<Manufacturer>();
        }

        #endregion
    }
}