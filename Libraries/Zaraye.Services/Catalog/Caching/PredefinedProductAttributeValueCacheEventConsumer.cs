﻿using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a predefined product attribute value cache event consumer
    /// </summary>
    public partial class PredefinedProductAttributeValueCacheEventConsumer : CacheEventConsumer<PredefinedProductAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(PredefinedProductAttributeValue entity)
        {
            await RemoveAsync(ZarayeCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, entity.ProductAttributeId);
        }
    }
}
