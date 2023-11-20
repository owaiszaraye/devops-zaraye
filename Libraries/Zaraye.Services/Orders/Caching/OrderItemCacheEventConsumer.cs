using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Orders.Caching
{
    /// <summary>
    /// Represents an order item cache event consumer
    /// </summary>
    public partial class OrderItemCacheEventConsumer : CacheEventConsumer<OrderItem>
    {
    }
}
