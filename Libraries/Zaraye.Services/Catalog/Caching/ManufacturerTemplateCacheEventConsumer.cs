using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a manufacturer template cache event consumer
    /// </summary>
    public partial class ManufacturerTemplateCacheEventConsumer : CacheEventConsumer<ManufacturerTemplate>
    {
    }
}
