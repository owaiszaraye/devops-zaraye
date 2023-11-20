using System.Threading.Tasks;
using Zaraye.Core.Domain.Gdpr;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR consent cache event consumer
    /// </summary>
    public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
    {
    }
}