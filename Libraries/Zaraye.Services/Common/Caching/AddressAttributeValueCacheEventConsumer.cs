﻿using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Common.Caching
{
    /// <summary>
    /// Represents a address attribute value cache event consumer
    /// </summary>
    public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(AddressAttributeValue entity)
        {
            await RemoveAsync(ZarayeCommonDefaults.AddressAttributeValuesByAttributeCacheKey, entity.AddressAttributeId);
        }
    }
}
