using System.Threading.Tasks;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Caching;
using Zaraye.Services.Discounts;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product cache event consumer
    /// </summary>
    public partial class ProductCacheEventConsumer : CacheEventConsumer<Product>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Product entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductManufacturersByProductPrefix, entity);
            await RemoveAsync(ZarayeCatalogDefaults.ProductsHomepageCacheKey);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductPricePrefix, entity);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductMultiplePricePrefix, entity);
            await RemoveByPrefixAsync(ZarayeEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.FeaturedProductIdsPrefix);

            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.ManufacturersByCategoryPrefix);
            }

            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Product), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
