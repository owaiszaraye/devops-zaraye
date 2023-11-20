using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product picture mapping cache event consumer
    /// </summary>
    public partial class ProductPictureCacheEventConsumer : CacheEventConsumer<ProductPicture>
    {
    }
}
