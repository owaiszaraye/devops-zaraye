using Zaraye.Core.Domain.Logging;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Logging.Caching
{
    /// <summary>
    /// Represents an activity log cache event consumer
    /// </summary>
    public partial class ActivityLogCacheEventConsumer : CacheEventConsumer<ActivityLog>
    {
    }
}