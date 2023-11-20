using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute option cache event consumer
    /// </summary>
    public partial class SpecificationAttributeOptionCacheEventConsumer : CacheEventConsumer<SpecificationAttributeOption>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(SpecificationAttributeOption entity, EntityEventType entityEventType)
        {
            await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);
            await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeOptionsCacheKey, entity.SpecificationAttributeId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.SpecificationAttributeGroupByProductPrefix);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
