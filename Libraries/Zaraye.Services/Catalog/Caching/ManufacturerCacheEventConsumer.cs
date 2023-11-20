using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;
using Zaraye.Services.Discounts;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a manufacturer cache event consumer
    /// </summary>
    public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Manufacturer entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(ZarayeDiscountDefaults.ManufacturerIdsPrefix);

            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.ManufacturersByCategoryPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity);

            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Manufacturer), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
