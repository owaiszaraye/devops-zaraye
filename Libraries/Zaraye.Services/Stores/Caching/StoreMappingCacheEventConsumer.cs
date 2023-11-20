using System.Threading.Tasks;
using Zaraye.Core.Domain.Stores;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Stores.Caching
{
    /// <summary>
    /// Represents a store mapping cache event consumer
    /// </summary>
    public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(StoreMapping entity)
        {
            await RemoveAsync(ZarayeStoreDefaults.StoreMappingsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(ZarayeStoreDefaults.StoreMappingIdsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(ZarayeStoreDefaults.StoreMappingExistsCacheKey, entity.EntityName);
        }
    }
}
