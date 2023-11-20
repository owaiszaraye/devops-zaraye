using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute group cache event consumer
    /// </summary>
    public partial class SpecificationAttributeGroupCacheEventConsumer : CacheEventConsumer<SpecificationAttributeGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(SpecificationAttributeGroup entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(ZarayeCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
        }
    }
}
