using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    public partial class CategoryTagCacheEventConsumer : CacheEventConsumer<CategoryTag>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(CategoryTag entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(ZarayeEntityCacheDefaults<CategoryTag>.Prefix);
        }
    }
}
