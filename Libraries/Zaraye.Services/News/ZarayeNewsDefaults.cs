using Zaraye.Core.Caching;

namespace Zaraye.Services.News
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class ZarayeNewsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static CacheKey NewsCommentsNumberCacheKey => new("Zaraye.newsitem.comments.number.{0}-{1}-{2}", NewsCommentsNumberPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// </remarks>
        public static string NewsCommentsNumberPrefix => "Zaraye.newsitem.comments.number.{0}";

        #endregion
    }
}