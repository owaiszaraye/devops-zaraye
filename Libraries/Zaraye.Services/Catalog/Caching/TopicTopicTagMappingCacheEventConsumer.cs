using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    public partial class TopicTopicTagMappingCacheEventConsumer : CacheEventConsumer<TopicTopicTagMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(TopicTopicTagMapping entity)
        {
            await RemoveAsync(ZarayeCatalogDefaults.IndustryTagsByIndustryCacheKey, entity.TopicId);
        }
    }
}
