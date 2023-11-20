using System.Threading.Tasks;
using Zaraye.Core.Domain.Directory;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Directory.Caching
{
    /// <summary>
    /// Represents a currency cache event consumer
    /// </summary>
    public partial class CurrencyCacheEventConsumer : CacheEventConsumer<Currency>
    {
    }
}
