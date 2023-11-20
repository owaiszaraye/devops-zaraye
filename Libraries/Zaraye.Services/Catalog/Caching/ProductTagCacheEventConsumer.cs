﻿using System.Threading.Tasks;
﻿using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product tag cache event consumer
    /// </summary>
    public partial class ProductTagCacheEventConsumer : CacheEventConsumer<ProductTag>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductTag entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(ZarayeEntityCacheDefaults<ProductTag>.Prefix);
        }
    }
}
