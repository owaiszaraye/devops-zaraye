﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a review type entity builder
    /// </summary>
    public partial class ReviewTypeBuilder : ZarayeEntityBuilder<ReviewType>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReviewType.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ReviewType.Description)).AsString(400).NotNullable();
        }

        #endregion
    }
}
