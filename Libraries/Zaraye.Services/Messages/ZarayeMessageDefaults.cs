using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Messages;

namespace Zaraye.Services.Messages
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class ZarayeMessageDefaults
    {
        /// <summary>
        /// Gets a key for notifications list from TempDataDictionary
        /// </summary>
        public static string NotificationListKey => "NotificationList";

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : is active?
        /// </remarks>
        public static CacheKey MessageTemplatesAllCacheKey => new("Zaraye.messagetemplate.all.{0}-{1}", ZarayeEntityCacheDefaults<MessageTemplate>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        public static CacheKey MessageTemplatesByNameCacheKey => new("Zaraye.messagetemplate.byname.{0}-{1}", MessageTemplatesByNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// </remarks>
        public static string MessageTemplatesByNamePrefix => "Zaraye.messagetemplate.byname.{0}";

        #endregion
    }
}