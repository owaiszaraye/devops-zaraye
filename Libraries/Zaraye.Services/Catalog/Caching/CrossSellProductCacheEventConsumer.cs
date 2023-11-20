using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a cross-sell product cache event consumer
    /// </summary>
    public partial class CrossSellProductCacheEventConsumer : CacheEventConsumer<CrossSellProduct>
    {
    }
}
