using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value entity builder
    /// </summary>
    public partial class PredefinedProductAttributeValueBuilder : ZarayeEntityBuilder<PredefinedProductAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PredefinedProductAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(PredefinedProductAttributeValue.ProductAttributeId)).AsInt32().ForeignKey<ProductAttribute>();
        }

        #endregion
    }
}