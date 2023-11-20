using System.Data;
using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Discounts
{
    /// <summary>
    /// Represents a discount requirement entity builder
    /// </summary>
    public partial class DiscountRequirementBuilder : ZarayeEntityBuilder<DiscountRequirement>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DiscountRequirement.DiscountId)).AsInt32().ForeignKey<Discount>()
                .WithColumn(nameof(DiscountRequirement.ParentId)).AsInt32().Nullable().ForeignKey<DiscountRequirement>(onDelete: Rule.None);
        }

        #endregion
    }
}