using Zaraye.Core.Domain.Shipping;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a product availability range cache event consumer
    /// </summary>
    public partial class ProductAvailabilityRangeCacheEventConsumer : CacheEventConsumer<ProductAvailabilityRange>
    {
    }
}