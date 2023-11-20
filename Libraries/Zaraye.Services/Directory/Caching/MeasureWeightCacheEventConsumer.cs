using System.Threading.Tasks;
using Zaraye.Core.Domain.Directory;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Directory.Caching
{
    /// <summary>
    /// Represents a measure weight cache event consumer
    /// </summary>
    public partial class MeasureWeightCacheEventConsumer : CacheEventConsumer<MeasureWeight>
    {
    }
}
