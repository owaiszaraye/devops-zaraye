using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;
using Zaraye.Framework.Mvc.Routing;

namespace Zaraye.Infrastructure
{
    /// <summary>
    /// Represents base provider
    /// </summary>
    public partial class BaseRouteProvider
    {
        /// <summary>
        /// Get pattern used to detect routes with language code
        /// </summary>
        /// <returns></returns>
        protected string GetLanguageRoutePattern()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //this pattern is set once at the application start, when we don't have the selected language yet
                    //so we use 'en' by default for the language value, later it'll be replaced with the working language code
                    var code = "en";
                    return $"{{{ZarayeRoutingDefaults.RouteValue.Language}:maxlength(2):{ZarayeRoutingDefaults.LanguageParameterTransformer}={code}}}";
                }
            }

            return string.Empty;
        }
    }
}