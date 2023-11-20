using Zaraye.Core.Caching;

namespace Zaraye.Services.Forums
{
    /// <summary>
    /// Represents default values related to forums services
    /// </summary>
    public static partial class ZarayeForumDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        public static CacheKey ForumByForumGroupCacheKey => new("Zaraye.forum.byforumgroup.{0}");

        #endregion
    }
}