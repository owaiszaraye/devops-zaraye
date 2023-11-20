using System.Threading.Tasks;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount cache event consumer
    /// </summary>
    public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Discount entity)
        {
            await RemoveAsync(ZarayeDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity);
            await RemoveByPrefixAsync(ZarayeDiscountDefaults.CategoryIdsByDiscountPrefix, entity);
            await RemoveByPrefixAsync(ZarayeDiscountDefaults.ManufacturerIdsByDiscountPrefix, entity);
            await RemoveByPrefixAsync(ZarayeDiscountDefaults.AppliedDiscountsCachePrefix);
        }
    }
}
