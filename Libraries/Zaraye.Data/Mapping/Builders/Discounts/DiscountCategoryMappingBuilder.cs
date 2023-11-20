using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Discounts
{
    /// <summary>
    /// Represents a discount category mapping entity builder
    /// </summary>
    public partial class DiscountCategoryMappingBuilder : ZarayeEntityBuilder<DiscountCategoryMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountCategoryMapping), nameof(DiscountCategoryMapping.DiscountId)))
                    .AsInt32().PrimaryKey().ForeignKey<Discount>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountCategoryMapping), nameof(DiscountCategoryMapping.EntityId)))
                    .AsInt32().PrimaryKey().ForeignKey<Category>();
        }

        #endregion
    }
}