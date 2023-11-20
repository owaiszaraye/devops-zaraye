using Zaraye.Core.Domain.Shipping;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipment item cache event consumer
    /// </summary>
    public partial class ShipmentItemCacheEventConsumer : CacheEventConsumer<ShipmentItem>
    {
    }
}