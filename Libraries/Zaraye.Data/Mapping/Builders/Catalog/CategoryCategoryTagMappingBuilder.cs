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
    public partial class CategoryCategoryTagMappingBuilder : ZarayeEntityBuilder<CategoryCategoryTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CategoryCategoryTagMapping), nameof(CategoryCategoryTagMapping.CategoryId)))
                    .AsInt32().PrimaryKey().ForeignKey<Category>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CategoryCategoryTagMapping), nameof(CategoryCategoryTagMapping.CategoryTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<CategoryTag>();
        }

        #endregion
    }
}
