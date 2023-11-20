using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders.Caching
{
    /// <summary>
    /// Represents a return request reason cache event consumer
    /// </summary>
    public partial class ReturnRequestReasonCacheEventConsumer : CacheEventConsumer<ReturnRequestReason>
    {
    }
}
