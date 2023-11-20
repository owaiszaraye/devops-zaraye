using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Orders.Caching
{
    /// <summary>
    /// Represents a recurring payment history cache event consumer
    /// </summary>
    public partial class RecurringPaymentHistoryCacheEventConsumer : CacheEventConsumer<RecurringPaymentHistory>
    {
    }
}
