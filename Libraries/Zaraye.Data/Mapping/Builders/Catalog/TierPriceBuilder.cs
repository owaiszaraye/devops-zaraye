using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Customers;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tier price entity builder
    /// </summary>
    public partial class TierPriceBuilder : ZarayeEntityBuilder<TierPrice>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TierPrice.CustomerRoleId)).AsInt32().Nullable().ForeignKey<CustomerRole>()
                .WithColumn(nameof(TierPrice.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}