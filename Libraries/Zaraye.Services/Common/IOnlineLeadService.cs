using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial interface IOnlineLeadService
    {
        #region Methods

         Task InsertOnlineLeadAsync(OnlineLead onlineLead);
         Task UpdateOnlineLeadAsync(OnlineLead onlineLead);
         Task DeleteOnlineLeadAsync(OnlineLead onlineLead);
        Task<IPagedList<OnlineLead>> GetAllOnlineLeadsAsync(
          string service = "", string contact = "", int countryId = 0, string city = "", DateTime? startDateUtc = null, DateTime? endDateUtc = null,
          bool showHidden = false, int pageIndex = 0, List<int> olsIds = null,
          int pageSize = int.MaxValue, bool getOnlyTotalCount = false, string source = "");
         Task<OnlineLead> GetOnlineLeadByIdAsync(int buyerRequirementId);

         Task<IList<OnlineLead>> GetOnlineLeadByIdsAsync(int[] ids);
        #endregion
    }
}