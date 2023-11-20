using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product review helpfulness cache event consumer
    /// </summary>
    public partial class ProductReviewHelpfulnessCacheEventConsumer : CacheEventConsumer<ProductReviewHelpfulness>
    {
    }
}
