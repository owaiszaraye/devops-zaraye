using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute cache event consumer
    /// </summary>
    public partial class SpecificationAttributeCacheEventConsumer : CacheEventConsumer<SpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(SpecificationAttribute entity, EntityEventType entityEventType)
        {
            await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);

            if (entityEventType != EntityEventType.Insert)
            {
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
            }

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
