using System.Threading.Tasks;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount requirement cache event consumer
    /// </summary>
    public partial class DiscountRequirementCacheEventConsumer : CacheEventConsumer<DiscountRequirement>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(DiscountRequirement entity)
        {
            await RemoveAsync(ZarayeDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity.DiscountId);
        }
    }
}
