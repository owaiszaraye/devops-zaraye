using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial class OnlineLeadService : IOnlineLeadService
    {
        #region Fileds
        private readonly IRepository<OnlineLead> _onlineLeadRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public OnlineLeadService(IRepository<OnlineLead> onlineLeadRepository,
            IWorkContext workContext)
        {
            _onlineLeadRepository = onlineLeadRepository;
            _workContext = workContext;
        }
        #endregion

        #region Methods

        public virtual async Task InsertOnlineLeadAsync(OnlineLead onlineLead)
        {
            onlineLead.CreatedById = (await _workContext.GetCurrentCustomerAsync()).Id;
            await _onlineLeadRepository.InsertAsync(onlineLead);
        }

        public virtual async Task UpdateOnlineLeadAsync(OnlineLead onlineLead)
        {
            await _onlineLeadRepository.UpdateAsync(onlineLead);
        }

        public virtual async Task DeleteOnlineLeadAsync(OnlineLead onlineLead)
        {
            onlineLead.DeletedById = (await _workContext.GetCurrentCustomerAsync()).Id;
            await _onlineLeadRepository.DeleteAsync(onlineLead);
        }

        public virtual async Task<IPagedList<OnlineLead>> GetAllOnlineLeadsAsync(
            string service = "", string contact = "", int countryId = 0, string city = "", DateTime? startDateUtc = null, DateTime? endDateUtc = null,
            bool showHidden = false, int pageIndex = 0, List<int> olsIds = null,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false,string source="")
        {
            var query = _onlineLeadRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (!string.IsNullOrWhiteSpace(service))
                query = query.Where(br => br.Service.Contains(service));
            if (!string.IsNullOrWhiteSpace(contact))
                query = query.Where(br => br.ContactNumber.Contains(contact));

            if (!string.IsNullOrWhiteSpace(source))
                query = query.Where(br => br.Source.Contains(source));

            if (countryId>0)
                query = query.Where(br => br.CountryId==countryId);

            if (olsIds != null && olsIds.Any())
                query = query.Where(br => olsIds.Contains(br.StatusId));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(br => br.CityName.Contains(city));

            if (startDateUtc.HasValue)
                query = query.Where(o => startDateUtc.Value <= o.CreatedOnUtc);
            if (endDateUtc.HasValue)
                query = query.Where(o => endDateUtc.Value >= o.CreatedOnUtc);

            query = query.OrderByDescending(br => br.CreatedOnUtc);

            var buyerRequirements = await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
            return buyerRequirements;
        }

        public virtual async Task<OnlineLead> GetOnlineLeadByIdAsync(int buyerRequirementId)
        {
            return await _onlineLeadRepository.GetByIdAsync(buyerRequirementId);
        }

        public virtual async Task<IList<OnlineLead>> GetOnlineLeadByIdsAsync(int[] ids)
        {
            return await _onlineLeadRepository.GetByIdsAsync(ids, includeDeleted: false);
        }

        #endregion
    }
}