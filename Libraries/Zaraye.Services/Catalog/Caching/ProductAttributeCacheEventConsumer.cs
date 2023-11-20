using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute cache event consumer
    /// </summary>
    public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductAttribute entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Insert)
                await RemoveAsync(ZarayeCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
