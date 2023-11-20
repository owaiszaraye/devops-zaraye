using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tier price cache event consumer
    /// </summary>
    public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(TierPrice entity)
        {
            await RemoveAsync(ZarayeCatalogDefaults.TierPricesByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
        }
    }
}
