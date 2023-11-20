using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Configuration;

namespace Zaraye.Services.Configuration
{
    /// <summary>
    /// Represents default values related to settings
    /// </summary>
    public static partial class ZarayeSettingsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new("Zaraye.setting.all.dictionary.", ZarayeEntityCacheDefaults<Setting>.Prefix);

        #endregion
    }
}