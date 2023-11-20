using Zaraye.Core.Domain.EmployeeInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Infrastructure;
using Zaraye.Core;
using Zaraye.Data;

namespace Zaraye.Services.Common
{
    public class EmployeeInsightsService : IEmployeeInsightsService
    {
        public virtual async Task DeleteEmployeeInsightsAsync(EmployeeInsights employeeInsights)
        {
            var _employeeInsightsRepository = EngineContext.Current.Resolve<IRepository<EmployeeInsights>>();
            await _employeeInsightsRepository.DeleteAsync(employeeInsights);
        }

        public virtual async Task<IPagedList<EmployeeInsights>> GetAllEmployeeInsightsAsync(bool? published = null, string title = "", bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var _employeeInsightsRepository = EngineContext.Current.Resolve<IRepository<EmployeeInsights>>();
            var query = _employeeInsightsRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (published.HasValue)
                query = query.Where(br => br.Published == published);

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(br => br.Title.Contains(title));

            query = query.OrderBy(br => br.DisplayOrder).ThenBy(X => X.Title);

            var employeeInsights = await query.ToPagedListAsync(pageIndex, pageSize);
            return employeeInsights;
        }

        public virtual async Task<EmployeeInsights> GetEmployeeInsightsByIdAsync(int appSliderId)
        {
            var _employeeInsightsRepository = EngineContext.Current.Resolve<IRepository<EmployeeInsights>>();
            return await _employeeInsightsRepository.GetByIdAsync(appSliderId, cache => default);
        }

        public virtual async Task InsertEmployeeInsightsAsync(EmployeeInsights employeeInsights)
        {
            var _employeeInsightsRepository = EngineContext.Current.Resolve<IRepository<EmployeeInsights>>();
            await _employeeInsightsRepository.InsertAsync(employeeInsights);
        }

        public virtual async Task UpdateEmployeeInsightsAsync(EmployeeInsights employeeInsights)
        {
            var _employeeInsightsRepository = EngineContext.Current.Resolve<IRepository<EmployeeInsights>>();
            await _employeeInsightsRepository.UpdateAsync(employeeInsights);
        }
    }
}
