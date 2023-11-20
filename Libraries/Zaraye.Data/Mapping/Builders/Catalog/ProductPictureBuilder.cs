using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Media;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product picture entity builder
    /// </summary>
    public partial class ProductPictureBuilder : ZarayeEntityBuilder<ProductPicture>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductPicture.PictureId)).AsInt32().ForeignKey<Picture>()
                .WithColumn(nameof(ProductPicture.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}