using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Directory;

namespace Zaraye.Services.Directory
{
    /// <summary>
    /// Represents default values related to directory services
    /// </summary>
    public static partial class ZarayeDirectoryDefaults
    {
        #region Caching defaults

        #region Countries

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Two letter ISO code
        /// </remarks>
        public static CacheKey CountriesByTwoLetterCodeCacheKey => new("Zaraye.country.bytwoletter.{0}", ZarayeEntityCacheDefaults<Country>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Two letter ISO code
        /// </remarks>
        public static CacheKey CountriesByThreeLetterCodeCacheKey => new("Zaraye.country.bythreeletter.{0}", ZarayeEntityCacheDefaults<Country>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : show hidden records?
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CountriesAllCacheKey => new("Zaraye.country.all.{0}-{1}-{2}", ZarayeEntityCacheDefaults<Country>.Prefix);

        #endregion

        #region Currencies

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CurrenciesAllCacheKey => new("Zaraye.currency.all.{0}", ZarayeEntityCacheDefaults<Currency>.AllPrefix);

        #endregion

        #region States and provinces

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : language ID
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey StateProvincesByCountryCacheKey => new("Zaraye.stateprovince.bycountry.{0}-{1}-{2}", ZarayeEntityCacheDefaults<StateProvince>.Prefix);
        public static CacheKey PriceDiscoveryStateProvincesByCountryCacheKey => new("Zaraye.stateprovince.bycountry.{0}", ZarayeEntityCacheDefaults<StateProvince>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey StateProvincesAllCacheKey => new("Zaraye.stateprovince.all.{0}", ZarayeEntityCacheDefaults<StateProvince>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : abbreviation
        /// {1} : country ID
        /// </remarks>
        public static CacheKey StateProvincesByAbbreviationCacheKey => new("Zaraye.stateprovince.byabbreviation.{0}-{1}", ZarayeEntityCacheDefaults<StateProvince>.Prefix);

        #endregion

        #endregion
    }
}
