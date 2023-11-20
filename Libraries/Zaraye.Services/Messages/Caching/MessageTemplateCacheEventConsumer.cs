using Zaraye.Core.Domain.Messages;
using Zaraye.Services.Caching;
using System.Threading.Tasks;

namespace Zaraye.Services.Messages.Caching
{
    /// <summary>
    /// Represents a message template cache event consumer
    /// </summary>
    public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(MessageTemplate entity)
        {
            await RemoveByPrefixAsync(ZarayeMessageDefaults.MessageTemplatesByNamePrefix, entity.Name);
        }
    }
}
