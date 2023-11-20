using Zaraye.Core.Domain.Configuration;
using Zaraye.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Services.Configuration
{
    public partial interface IAppVersionService
    {
        #region Methods

        Task InsertAppVersionAsync(AppVersion appVersion);

        Task UpdateAppVersionAsync(AppVersion appVersion);

        Task DeleteAppVersionAsync(AppVersion appVersion);

        Task<IPagedList<AppVersion>> GetAllAppVersionsAsync(string type = "", string platform = "", bool onlyForceUpdate = false, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue);

        Task<AppVersion> GetAppVersionByIdAsync(int appVersionId);

        Task<AppVersion> FindAppVersion(string version, string platform, string type);

        #endregion
    }
}
