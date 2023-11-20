using Zaraye.Core.Domain.Customers;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Customers.Caching
{
    /// <summary>
    /// Represents an external authentication record cache event consumer
    /// </summary>
    public partial class ExternalAuthenticationRecordCacheEventConsumer : CacheEventConsumer<ExternalAuthenticationRecord>
    {
    }
}
