using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-product mapping cache event consumer
    /// </summary>
    public partial class DiscountProductMappingCacheEventConsumer : CacheEventConsumer<DiscountProductMapping>
    {
        protected override async Task ClearCacheAsync(DiscountProductMapping entity)
        {
            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Product), entity.EntityId);

            await base.ClearCacheAsync(entity);
        }
    }
}