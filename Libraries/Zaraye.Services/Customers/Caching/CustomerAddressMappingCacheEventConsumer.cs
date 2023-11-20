using System.Threading.Tasks;
using Zaraye.Core.Domain.Customers;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Customers.Caching
{
    /// <summary>
    /// Represents a customer address mapping cache event consumer
    /// </summary>
    public partial class CustomerAddressMappingCacheEventConsumer : CacheEventConsumer<CustomerAddressMapping>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(CustomerAddressMapping entity, EntityEventType entityEventType)
        {
            await RemoveAsync(ZarayeCustomerServicesDefaults.CustomerAddressesCacheKey, entity.CustomerId);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(ZarayeCustomerServicesDefaults.CustomerAddressCacheKey, entity.CustomerId, entity.AddressId);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}