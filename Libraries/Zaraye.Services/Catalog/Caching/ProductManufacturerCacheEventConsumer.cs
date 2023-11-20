using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product manufacturer cache event consumer
    /// </summary>
    public partial class ProductManufacturerCacheEventConsumer : CacheEventConsumer<ProductManufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductManufacturer entity)
        {
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductManufacturersByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ManufacturerFeaturedProductIdsPrefix, entity.ManufacturerId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ManufacturersByCategoryPrefix);
            await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity.ManufacturerId.ToString());
        }
    }
}
