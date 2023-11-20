using Zaraye.Core.Domain.EmployeeInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core;

namespace Zaraye.Services.Common
{
    public interface IEmployeeInsightsService
    {

        Task DeleteEmployeeInsightsAsync(EmployeeInsights employeeInsights);

        Task<EmployeeInsights> GetEmployeeInsightsByIdAsync(int employeeInsightsId);

        Task<IPagedList<EmployeeInsights>> GetAllEmployeeInsightsAsync(bool? published = null,
            string title = "", bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        Task InsertEmployeeInsightsAsync(EmployeeInsights employeeInsights);

        Task UpdateEmployeeInsightsAsync(EmployeeInsights employeeInsights);
    }
}
