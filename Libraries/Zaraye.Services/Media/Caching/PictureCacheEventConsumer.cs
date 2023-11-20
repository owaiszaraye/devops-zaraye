using Zaraye.Core.Domain.Media;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Media.Caching
{
    /// <summary>
    /// Represents a picture cache event consumer
    /// </summary>
    public partial class PictureCacheEventConsumer : CacheEventConsumer<Picture>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Picture entity)
        {
            await RemoveByPrefixAsync(ZarayeMediaDefaults.ThumbsExistsPrefix);
        }
    }
}
