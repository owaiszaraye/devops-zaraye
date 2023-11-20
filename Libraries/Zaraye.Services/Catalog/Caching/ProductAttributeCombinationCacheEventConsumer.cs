using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute combination cache event consumer
    /// </summary>
    public partial class ProductAttributeCombinationCacheEventConsumer : CacheEventConsumer<ProductAttributeCombination>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductAttributeCombination entity)
        {
            await RemoveAsync(ZarayeCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
            await RemoveAsync(ZarayeCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
        }
    }
}
