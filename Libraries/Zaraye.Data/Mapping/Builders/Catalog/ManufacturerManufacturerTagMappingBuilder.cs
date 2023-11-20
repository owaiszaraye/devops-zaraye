using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class ManufacturerManufacturerTagMappingBuilder : ZarayeEntityBuilder<ManufacturerManufacturerTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ManufacturerManufacturerTagMapping), nameof(ManufacturerManufacturerTagMapping.ManufacturerId)))
                    .AsInt32().PrimaryKey().ForeignKey<Manufacturer>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ManufacturerManufacturerTagMapping), nameof(ManufacturerManufacturerTagMapping.ManufacturerTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<ManufacturerTag>();
        }

        #endregion
    }
}
