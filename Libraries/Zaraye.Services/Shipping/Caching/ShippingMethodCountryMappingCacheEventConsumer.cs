using Zaraye.Core.Domain.Shipping;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipping method-country mapping cache event consumer
    /// </summary>
    public partial class ShippingMethodCountryMappingCacheEventConsumer : CacheEventConsumer<ShippingMethodCountryMapping>
    {
    }
}