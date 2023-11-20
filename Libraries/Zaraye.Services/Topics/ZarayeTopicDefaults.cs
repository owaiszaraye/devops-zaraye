using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Topics;

namespace Zaraye.Services.Topics
{
    /// <summary>
    /// Represents default values related to topic services
    /// </summary>
    public static partial class ZarayeTopicDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// </remarks>
        public static CacheKey TopicsAllCacheKey => new("Zaraye.topic.all.{0}-{1}-{2}", ZarayeEntityCacheDefaults<Topic>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// {3} : customer role IDs hash
        /// </remarks>
        public static CacheKey TopicsAllWithACLCacheKey => new("Zaraye.topic.all.withacl.{0}-{1}-{2}-{3}", ZarayeEntityCacheDefaults<Topic>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : store id
        /// {2} : customer roles Ids hash
        /// </remarks>
        public static CacheKey TopicBySystemNameCacheKey => new("Zaraye.topic.bysystemname.{0}-{1}-{2}", TopicBySystemNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// </remarks>
        public static string TopicBySystemNamePrefix => "Zaraye.topic.bysystemname.{0}";

        #endregion
    }
}