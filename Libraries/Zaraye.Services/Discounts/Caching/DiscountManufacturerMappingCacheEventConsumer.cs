using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-manufacturer mapping cache event consumer
    /// </summary>
    public partial class DiscountManufacturerMappingCacheEventConsumer : CacheEventConsumer<DiscountManufacturerMapping>
    {
        protected override async Task ClearCacheAsync(DiscountManufacturerMapping entity)
        {
            await RemoveAsync(ZarayeDiscountDefaults.AppliedDiscountsCacheKey, nameof(Manufacturer), entity.EntityId);

            await base.ClearCacheAsync(entity);
        }
    }
}