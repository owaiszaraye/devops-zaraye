using Zaraye.Core.Domain.Customers;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Customers.Caching
{
    /// <summary>
    /// Represents a reward point history cache event consumer
    /// </summary>
    public partial class RewardPointsHistoryCacheEventConsumer : CacheEventConsumer<RewardPointsHistory>
    {
    }
}
