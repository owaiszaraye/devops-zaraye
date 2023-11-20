using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;
using Zaraye.Services.Discounts;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a category cache event consumer
    /// </summary>
    public partial class CategoryCacheEventConsumer : CacheEventConsumer<Category>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Category entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesByIndustryPrefix, entity);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesChildIdsPrefix, entity);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesHomepagePrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoryBreadcrumbPrefix);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefixAsync(ZarayeDiscountDefaults.CategoryIdsPrefix);

            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesByIndustryPrefix, entity);
            await RemoveByPrefixAsync(ZarayeCatalogDefaults.CategoriesByIndustryPrefix, entity.IndustryId);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(ZarayeCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, entity);

            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
