using System.Threading.Tasks;
using Zaraye.Core.Domain.Customers;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Customers.Caching
{
    /// <summary>
    /// Represents a customer attribute cache event consumer
    /// </summary>
    public partial class CustomerAttributeCacheEventConsumer : CacheEventConsumer<CustomerAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(CustomerAttribute entity)
        {
            await RemoveAsync(ZarayeCustomerServicesDefaults.CustomerAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
