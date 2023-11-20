using Zaraye.Core.Domain.ScheduleTasks;
using Zaraye.Services.Caching;

namespace Zaraye.Services.ScheduleTasks.Caching
{
    /// <summary>
    /// Represents a schedule task cache event consumer
    /// </summary>
    public partial class ScheduleTaskCacheEventConsumer : CacheEventConsumer<ScheduleTask>
    {
    }
}
