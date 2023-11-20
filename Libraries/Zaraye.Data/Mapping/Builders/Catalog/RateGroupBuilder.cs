using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class RateGroupBuilder : ZarayeEntityBuilder<RateGroup>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RateGroup.IndustryId)).AsInt32().ForeignKey<Industry>()
                .WithColumn(nameof(RateGroup.CategoryId)).AsInt32().ForeignKey<Category>()
                .WithColumn(nameof(RateGroup.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(RateGroup.BrandId)).AsInt32().ForeignKey<Manufacturer>();
        }

        #endregion
    }
}