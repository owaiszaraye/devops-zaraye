﻿using Zaraye.Core.Caching;
using Zaraye.Services.ScheduleTasks;

namespace Zaraye.Services.Caching
{
    /// <summary>
    /// Clear cache scheduled task implementation
    /// </summary>
    public partial class ClearCacheTask : IScheduleTask
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ClearCacheTask(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _staticCacheManager.ClearAsync();
        }

        #endregion
    }
}