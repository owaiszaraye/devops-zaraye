using Zaraye.Core.Domain.Logging;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Logging.Caching
{
    /// <summary>
    /// Represents a log cache event consumer
    /// </summary>
    public partial class LogCacheEventConsumer : CacheEventConsumer<Log>
    {
    }
}
