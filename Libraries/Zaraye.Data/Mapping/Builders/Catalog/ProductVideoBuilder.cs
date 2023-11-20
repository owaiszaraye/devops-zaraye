using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Media;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product video mapping entity builder
    /// </summary>
    public partial class ProductVideoBuilder : ZarayeEntityBuilder<ProductVideo>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductVideo.VideoId)).AsInt32().ForeignKey<Video>()
                .WithColumn(nameof(ProductVideo.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}
