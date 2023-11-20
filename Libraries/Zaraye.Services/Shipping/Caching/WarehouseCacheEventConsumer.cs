using Zaraye.Core.Domain.Shipping;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a warehouse cache event consumer
    /// </summary>
    public partial class WarehouseCacheEventConsumer : CacheEventConsumer<Warehouse>
    {
    }
}
