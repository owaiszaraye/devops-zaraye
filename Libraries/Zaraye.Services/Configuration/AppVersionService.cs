using Zaraye.Core.Domain.Configuration;
using Zaraye.Core;
using Zaraye.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Services.Configuration
{
    public partial class AppVersionService : IAppVersionService
    {
        #region Fields

        private readonly IRepository<AppVersion> _appVersionRepository;

        #endregion

        #region Ctor

        public AppVersionService(IRepository<AppVersion> appVersionRepository)
        {
            _appVersionRepository = appVersionRepository;
        }

        #endregion

        #region Methods

        public virtual async Task InsertAppVersionAsync(AppVersion appVersion)
        {
            await _appVersionRepository.InsertAsync(appVersion);
        }

        public virtual async Task UpdateAppVersionAsync(AppVersion appVersion)
        {
            await _appVersionRepository.UpdateAsync(appVersion);
        }

        public virtual async Task DeleteAppVersionAsync(AppVersion appVersion)
        {
            appVersion.Deleted = true;
            await _appVersionRepository.UpdateAsync(appVersion);
        }

        public virtual async Task<IPagedList<AppVersion>> GetAllAppVersionsAsync(string type = "", string platform = "", bool onlyForceUpdate = false, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _appVersionRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(br => br.Type == type);

            if (!string.IsNullOrWhiteSpace(platform))
                query = query.Where(br => br.Platform == platform);

            if (onlyForceUpdate)
                query = query.Where(br => br.ForceUpdate);

            if (!showHidden)
                query = query.Where(br => br.Published);

            query = query.OrderByDescending(br => br.CreatedOnUtc);

            var appVersions = await query.ToPagedListAsync(pageIndex, pageSize);
            return appVersions;
        }

        public virtual async Task<AppVersion> GetAppVersionByIdAsync(int appVersionId)
        {
            return await _appVersionRepository.GetByIdAsync(appVersionId);
        }

        public virtual async Task<AppVersion> FindAppVersion(string version, string platform, string type)
        {
            var query = _appVersionRepository.Table;
            var data = await query.FirstOrDefaultAsync(record => record.Version == version && record.Platform == platform && record.Type == type && !record.Deleted);
            return data;
        }

        #endregion
    }
}
