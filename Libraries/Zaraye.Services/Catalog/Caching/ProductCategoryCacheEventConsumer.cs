using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product category cache event consumer
    /// </summary>
    public partial class ProductCategoryCacheEventConsumer : CacheEventConsumer<ProductCategory>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductCategory entity)
        {
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductCategoriesByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoryFeaturedProductsIdsPrefix, entity.CategoryId);
            await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, entity.CategoryId.ToString());
            await RemoveAsync(ZarayeCatalogDefaults.ManufacturersByCategoryCacheKey, entity.CategoryId.ToString());
        }
    }
}
