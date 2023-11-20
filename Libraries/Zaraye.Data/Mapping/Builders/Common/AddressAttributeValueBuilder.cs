﻿using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Common;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Common
{
    /// <summary>
    /// Represents an address attribute value entity builder
    /// </summary>
    public partial class AddressAttributeValueBuilder : ZarayeEntityBuilder<AddressAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AddressAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(AddressAttributeValue.AddressAttributeId)).AsInt32().ForeignKey<AddressAttribute>();
        }

        #endregion
    }
}