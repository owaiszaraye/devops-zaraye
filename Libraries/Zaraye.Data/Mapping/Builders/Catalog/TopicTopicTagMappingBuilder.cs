using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Topics;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Catalog
{
    public partial class TopicTopicTagMappingBuilder : ZarayeEntityBuilder<TopicTopicTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TopicTopicTagMapping), nameof(TopicTopicTagMapping.TopicId)))
                    .AsInt32().PrimaryKey().ForeignKey<Topic>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(TopicTopicTagMapping), nameof(TopicTopicTagMapping.TopicTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<TopicTag>();
        }

        #endregion
    }
}
