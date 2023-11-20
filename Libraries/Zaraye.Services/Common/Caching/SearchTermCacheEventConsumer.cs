using Zaraye.Core.Domain.Common;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Common.Caching
{
    /// <summary>
    /// Represents a search term cache event consumer
    /// </summary>
    public partial class SearchTermCacheEventConsumer : CacheEventConsumer<SearchTerm>
    {
    }
}
