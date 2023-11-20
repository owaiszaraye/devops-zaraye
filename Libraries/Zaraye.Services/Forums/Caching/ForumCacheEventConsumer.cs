using System.Threading.Tasks;
using Zaraye.Core.Domain.Forums;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum cache event consumer
    /// </summary>
    public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Forum entity)
        {
            await RemoveAsync(ZarayeForumDefaults.ForumByForumGroupCacheKey, entity.ForumGroupId);
        }
    }
}
