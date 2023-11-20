using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders.Caching
{
    /// <summary>
    /// Represents a shopping cart item cache event consumer
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : CacheEventConsumer<ShoppingCartItem>
    {
    }
}
