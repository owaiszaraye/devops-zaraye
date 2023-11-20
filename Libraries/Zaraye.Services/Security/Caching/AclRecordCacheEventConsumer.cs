using System.Threading.Tasks;
using Zaraye.Core.Domain.Security;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Security.Caching
{
    /// <summary>
    /// Represents a ACL record cache event consumer
    /// </summary>
    public partial class AclRecordCacheEventConsumer : CacheEventConsumer<AclRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(AclRecord entity)
        {
            await RemoveAsync(ZarayeSecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(ZarayeSecurityDefaults.EntityAclRecordExistsCacheKey, entity.EntityName);
        }
    }
}
