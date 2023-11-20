using Zaraye.Core.Domain.Gdpr;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR log cache event consumer
    /// </summary>
    public partial class GdprLogCacheEventConsumer : CacheEventConsumer<GdprLog>
    {
    }
}
