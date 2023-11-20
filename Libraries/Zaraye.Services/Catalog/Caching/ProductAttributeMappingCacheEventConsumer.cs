using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute mapping cache event consumer
    /// </summary>
    public partial class ProductAttributeMappingCacheEventConsumer : CacheEventConsumer<ProductAttributeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductAttributeMapping entity)
        {
            await RemoveAsync(ZarayeCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
            await RemoveAsync(ZarayeCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);
            await RemoveAsync(ZarayeCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
        }
    }
}
