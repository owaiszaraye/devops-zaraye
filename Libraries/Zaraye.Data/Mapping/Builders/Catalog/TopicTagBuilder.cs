using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class TopicTagBuilder : ZarayeEntityBuilder<TopicTag>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(TopicTag.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}
