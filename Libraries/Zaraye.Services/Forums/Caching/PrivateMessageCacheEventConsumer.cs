using Zaraye.Core.Domain.Forums;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Forums.Caching
{
    /// <summary>
    /// Represents a private message cache event consumer
    /// </summary>
    public partial class PrivateMessageCacheEventConsumer : CacheEventConsumer<PrivateMessage>
    {
    }
}
