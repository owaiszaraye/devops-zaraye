using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;
using Zaraye.Services.Discounts;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a category cache event consumer
    /// </summary>
    public partial class IndustryCacheEventConsumer : CacheEventConsumer<Industry>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Industry entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.IndustriesHomepagePrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.IndustryBreadcrumbPrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesByIndustryPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, entity);

            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
