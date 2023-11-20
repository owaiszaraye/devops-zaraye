using Zaraye.Core.Caching;

namespace Zaraye.Services.Stores
{
    /// <summary>
    /// Represents default values related to stores services
    /// </summary>
    public static partial class ZarayeStoreDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingIdsCacheKey => new("Zaraye.storemapping.ids.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static CacheKey StoreMappingsCacheKey => new("Zaraye.storemapping.{0}-{1}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity name
        /// </remarks>
        public static CacheKey StoreMappingExistsCacheKey => new("Zaraye.storemapping.exists.{0}");

        #endregion
    }
}