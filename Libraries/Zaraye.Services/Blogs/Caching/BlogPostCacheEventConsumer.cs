using System.Threading.Tasks;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Blogs.Caching
{
    /// <summary>
    /// Represents a blog post cache event consumer
    /// </summary>
    public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(BlogPost entity)
        {
           await RemoveByPrefixAsync(ZarayeBlogsDefaults.BlogTagsPrefix);
        }
    }
}