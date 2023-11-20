using Zaraye.Core.Domain.Forums;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum subscription cache event consumer
    /// </summary>
    public partial class ForumSubscriptionCacheEventConsumer : CacheEventConsumer<ForumSubscription>
    {
    }
}
