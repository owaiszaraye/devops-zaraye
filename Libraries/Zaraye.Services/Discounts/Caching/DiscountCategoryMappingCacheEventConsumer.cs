using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-category mapping cache event consumer
    /// </summary>
    public partial class DiscountCategoryMappingCacheEventConsumer : CacheEventConsumer<DiscountCategoryMapping>
    {
        protected override async Task ClearCacheAsync(DiscountCategoryMapping entity)
        {
            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity.EntityId);
            
            await base.ClearCacheAsync(entity);
        }
    }
}