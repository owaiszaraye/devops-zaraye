using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Orders.Caching
{
    /// <summary>
    /// Represents a order cache event consumer
    /// </summary>
    public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
    {
    }
}