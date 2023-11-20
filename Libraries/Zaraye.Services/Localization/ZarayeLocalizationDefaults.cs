using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Localization;

namespace Zaraye.Services.Localization
{
    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class ZarayeLocalizationDefaults
    {
        #region Locales

        /// <summary>
        /// Gets a prefix of locale resources for the admin area
        /// </summary>
        public static string AdminLocaleStringResourcesPrefix => "Admin.";

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public static string EnumLocaleStringResourcesPrefix => "Enums.";

        /// <summary>
        /// Gets a prefix of locale resources for permissions 
        /// </summary>
        public static string PermissionLocaleStringResourcesPrefix => "Permission.";

        /// <summary>
        /// Gets a prefix of locale resources for plugin friendly names 
        /// </summary>
        public static string PluginNameLocaleStringResourcesPrefix => "Plugins.FriendlyName.";

        #endregion

        #region Caching defaults

        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey LanguagesAllCacheKey => new("Zaraye.language.all.{0}-{1}", LanguagesByStorePrefix, ZarayeEntityCacheDefaults<Language>.AllPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static string LanguagesByStorePrefix => "Zaraye.language.all.{0}";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllPublicCacheKey => new("Zaraye.localestringresource.bylanguage.public.{0}", ZarayeEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllAdminCacheKey => new("Zaraye.localestringresource.bylanguage.admin.{0}", ZarayeEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllCacheKey => new("Zaraye.localestringresource.bylanguage.{0}", ZarayeEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static CacheKey LocaleStringResourcesByNameCacheKey => new("Zaraye.localestringresource.byname.{0}-{1}", LocaleStringResourcesByNamePrefix, ZarayeEntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByNamePrefix => "Zaraye.localestringresource.byname.{0}";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertyCacheKey => new("Zaraye.localizedproperty.value.{0}-{1}-{2}-{3}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : locale key group
        /// {2} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertiesCacheKey => new("Zaraye.localizedproperty.all.{0}-{1}-{2}");

        #endregion

        #endregion
    }
}