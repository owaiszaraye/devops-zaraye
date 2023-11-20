using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Stores;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a blog comment entity builder
    /// </summary>
    public partial class BlogCommentBuilder : ZarayeEntityBuilder<BlogComment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BlogComment.StoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(BlogComment.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(BlogComment.BlogPostId)).AsInt32().ForeignKey<BlogPost>();
        }

        #endregion
    }
}