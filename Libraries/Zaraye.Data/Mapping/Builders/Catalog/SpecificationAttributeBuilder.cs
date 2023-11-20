﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a specification attribute entity builder
    /// </summary>
    public partial class SpecificationAttributeBuilder : ZarayeEntityBuilder<SpecificationAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(SpecificationAttribute.Name)).AsString(int.MaxValue).NotNullable();
            table.WithColumn(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).AsInt32().Nullable().ForeignKey<SpecificationAttributeGroup>();
        }

        #endregion
    }
}