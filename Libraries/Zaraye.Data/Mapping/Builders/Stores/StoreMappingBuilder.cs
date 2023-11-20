using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Stores;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Stores
{
    /// <summary>
    /// Represents a store mapping entity builder
    /// </summary>
    public partial class StoreMappingBuilder : ZarayeEntityBuilder<StoreMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StoreMapping.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(StoreMapping.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}