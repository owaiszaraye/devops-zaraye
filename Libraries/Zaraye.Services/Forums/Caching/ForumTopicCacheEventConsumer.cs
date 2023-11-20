using Zaraye.Core.Domain.Forums;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum topic cache event consumer
    /// </summary>
    public partial class ForumTopicCacheEventConsumer : CacheEventConsumer<ForumTopic>
    {
    }
}
