using Zaraye.Core.Domain.Common;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial interface IDashboardService
    {
        #region Method

        Task<DashboardDataModel> GetAllDashboarDataAsync();

        #endregion
    }
}