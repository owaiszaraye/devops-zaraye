using Zaraye.Core.Domain.Messages;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Messages.Caching
{
    /// <summary>
    /// Represents a campaign cache event consumer
    /// </summary>
    public partial class CampaignCacheEventConsumer : CacheEventConsumer<Campaign>
    {
    }
}
