using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product specification attribute cache event consumer
    /// </summary>
    public partial class ProductSpecificationAttributeCacheEventConsumer : CacheEventConsumer<ProductSpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductSpecificationAttribute entity)
        {
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductSpecificationAttributeByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
            await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, entity.ProductId);
        }
    }
}
