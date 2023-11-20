using System.Threading.Tasks;
using Zaraye.Core.Domain.Customers;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Customers.Caching
{
    /// <summary>
    /// Represents a customer role cache event consumer
    /// </summary>
    public partial class CustomerRoleCacheEventConsumer : CacheEventConsumer<CustomerRole>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(CustomerRole entity)
        {
            await RemoveByPrefixAsync(ZarayeCustomerServicesDefaults.CustomerRolesBySystemNamePrefix);
            await RemoveByPrefixAsync(ZarayeCustomerServicesDefaults.CustomerCustomerRolesPrefix);
        }
    }
}
