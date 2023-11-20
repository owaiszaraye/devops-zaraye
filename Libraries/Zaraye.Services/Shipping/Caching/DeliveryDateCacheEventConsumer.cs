using Zaraye.Core.Domain.Shipping;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a delivery date cache event consumer
    /// </summary>
    public partial class DeliveryDateCacheEventConsumer : CacheEventConsumer<DeliveryDate>
    {
    }
}