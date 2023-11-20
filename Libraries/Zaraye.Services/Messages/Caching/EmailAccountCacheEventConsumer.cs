using Zaraye.Core.Domain.Messages;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Messages.Caching
{
    /// <summary>
    /// Represents an email account cache event consumer
    /// </summary>
    public partial class EmailAccountCacheEventConsumer : CacheEventConsumer<EmailAccount>
    {
    }
}
