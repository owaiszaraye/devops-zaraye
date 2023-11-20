using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using System;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial interface IAppFeedBackService
    {
        Task InsertAppFeedBackAsync(AppFeedBack appFeedBack);

        Task UpdateAppFeedBackAsync(AppFeedBack appFeedBack);

        Task<AppFeedBack> GetFeedBackByIdAsync(int feedBackId);

        Task<IPagedList<AppFeedBack>> GetAllAppFeedBackAsync(string searchFeedBack = "",
            string searchOwnerEmail = "", string searchUserEmail = "", string searchFullName = "",
            string searchUsername = "", DateTime? searchStartDate = null, DateTime? searchEndDate = null, int pageIndex = 0, int pageSize = int.MaxValue);     
    }
}