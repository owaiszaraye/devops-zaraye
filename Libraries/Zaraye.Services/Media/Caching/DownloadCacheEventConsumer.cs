using Zaraye.Core.Domain.Media;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Media.Caching
{
    /// <summary>
    /// Represents a download cache event consumer
    /// </summary>
    public partial class DownloadCacheEventConsumer : CacheEventConsumer<Download>
    {
    }
}
