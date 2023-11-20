using Zaraye.Core.Domain.Localization;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Localization.Caching
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(LocaleStringResource entity)
        {
            await RemoveAsync(ZarayeLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
            await RemoveAsync(ZarayeLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
            await RemoveAsync(ZarayeLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
            await RemoveByPrefixAsync(ZarayeLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
        }
    }
}
