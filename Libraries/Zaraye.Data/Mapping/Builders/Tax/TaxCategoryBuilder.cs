using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Tax;

namespace Zaraye.Data.Mapping.Builders.Tax
{
    /// <summary>
    /// Represents tax category builder
    /// </summary>
    public partial class TaxCategoryBuilder : ZarayeEntityBuilder<TaxCategory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(TaxCategory.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}