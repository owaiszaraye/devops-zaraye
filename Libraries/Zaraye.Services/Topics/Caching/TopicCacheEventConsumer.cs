using System.Threading.Tasks;
using Zaraye.Core.Domain.Topics;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic cache event consumer
    /// </summary>
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Topic entity)
        {
            await RemoveByPrefixAsync(ZarayeTopicDefaults.TopicBySystemNamePrefix, entity.SystemName);
        }
    }
}
