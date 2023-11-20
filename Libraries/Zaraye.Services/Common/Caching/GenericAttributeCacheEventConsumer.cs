﻿using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Common.Caching
{
    /// <summary>
    /// Represents a generic attribute cache event consumer
    /// </summary>
    public partial class GenericAttributeCacheEventConsumer : CacheEventConsumer<GenericAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(GenericAttribute entity)
        {
            await RemoveAsync(ZarayeCommonDefaults.GenericAttributeCacheKey, entity.EntityId, entity.KeyGroup);
        }
    }
}
