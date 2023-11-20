using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Stores;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.News
{
    /// <summary>
    /// Represents a news comment entity builder
    /// </summary>
    public partial class NewsCommentBuilder : ZarayeEntityBuilder<NewsComment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(NewsComment.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(NewsComment.NewsItemId)).AsInt32().ForeignKey<NewsItem>()
                .WithColumn(nameof(NewsComment.StoreId)).AsInt32().ForeignKey<Store>();
        }

        #endregion
    }
}