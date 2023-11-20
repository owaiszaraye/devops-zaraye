using Zaraye.Core.Domain.Localization;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Localization.Caching
{
    /// <summary>
    /// Represents a language cache event consumer
    /// </summary>
    public partial class LanguageCacheEventConsumer : CacheEventConsumer<Language>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Language entity)
        {
            await RemoveAsync(ZarayeLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity);
            await RemoveAsync(ZarayeLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity);
            await RemoveAsync(ZarayeLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity);
            await RemoveByPrefixAsync(ZarayeLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity);
        }
    }
}