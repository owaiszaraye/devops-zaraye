﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Shipping;

namespace Zaraye.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a delivery date entity builder
    /// </summary>
    public partial class DeliveryDateBuilder : ZarayeEntityBuilder<DeliveryDate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(DeliveryDate.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}