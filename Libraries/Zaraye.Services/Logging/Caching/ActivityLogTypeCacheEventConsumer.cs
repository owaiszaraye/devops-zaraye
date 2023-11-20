using Zaraye.Core.Domain.Logging;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Logging.Caching
{
    /// <summary>
    /// Represents a activity log type cache event consumer
    /// </summary>
    public partial class ActivityLogTypeCacheEventConsumer : CacheEventConsumer<ActivityLogType>
    {
    }
}
