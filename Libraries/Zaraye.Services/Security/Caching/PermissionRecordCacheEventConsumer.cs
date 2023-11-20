using Zaraye.Core.Domain.Security;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Security.Caching
{
    /// <summary>
    /// Represents a permission record cache event consumer
    /// </summary>
    public partial class PermissionRecordCacheEventConsumer : CacheEventConsumer<PermissionRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(PermissionRecord entity)
        {
            await RemoveByPrefixAsync(ZarayeSecurityDefaults.PermissionAllowedPrefix, entity.SystemName);
        }
    }
}
