using System.Threading.Tasks;
using Zaraye.Core.Domain.Topics;
using Zaraye.Services.Caching;

namespace Zaraye.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
    }
}
