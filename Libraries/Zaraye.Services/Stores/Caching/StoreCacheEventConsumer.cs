using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Stores;
using Zaraye.Services.Caching;
using Zaraye.Services.Localization;
using System.Threading.Tasks;

namespace Zaraye.Services.Stores.Caching
{
    /// <summary>
    /// Represents a store cache event consumer
    /// </summary>
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Store entity)
        {
            await RemoveByPrefixAsync(ZarayeEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(ZarayeLocalizationDefaults.LanguagesByStorePrefix, entity);
        }
    }
}
