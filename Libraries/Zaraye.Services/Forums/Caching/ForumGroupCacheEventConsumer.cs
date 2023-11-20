using Zaraye.Core.Domain.Forums;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum group cache event consumer
    /// </summary>
    public partial class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ForumGroup entity)
        {
            await RemoveAsync(ZarayeForumDefaults.ForumByForumGroupCacheKey, entity);
        }
    }
}
