using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data;
using Zaraye.Services.Common;
using Zaraye.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders
{
    public partial class QuotationService : IQuotationService
    {
        #region Fields

        private IRepository<Quotation> _quotationRepository;
        private readonly IBidRequestTrackerService _bidRequestTrackerService;
        private readonly IWorkContext _workContext;
        private IRepository<Request> _buyerRequestRepository;
        private IRepository<RequestForQuotation> _requestForQuotationRepository;
        private readonly IRepository<SupplierProduct> _supplierProductRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BidRequestTracker> _bidRequestTrackerRepository;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomNumberFormatter _customNumberFormatter;

        #endregion

        #region Ctor

        public QuotationService(
            IRepository<Quotation> supplierBidRepository,

            IWorkContext workContext,
            IBidRequestTrackerService bidRequestTrackerService,
            IRepository<Request> buyerRequestRepository,
            IRepository<SupplierProduct> supplierProductRepository,
            IRepository<Customer> customerRepository,
            IRepository<BidRequestTracker> bidRequestTrackerRepository,
            IDateTimeHelper dateTimeHelper,
            ICustomNumberFormatter customNumberFormatter,
            IRepository<RequestForQuotation> requestForQuotationRepository)
        {
            _quotationRepository = supplierBidRepository;
            _workContext = workContext;
            _bidRequestTrackerService = bidRequestTrackerService;
            _buyerRequestRepository = buyerRequestRepository;
            _supplierProductRepository = supplierProductRepository;
            _customerRepository = customerRepository;
            _bidRequestTrackerRepository = bidRequestTrackerRepository;
            _dateTimeHelper = dateTimeHelper;
            _customNumberFormatter = customNumberFormatter;
            _requestForQuotationRepository = requestForQuotationRepository;
        }

        #endregion

        #region Methods

        public virtual async Task InsertQuotationAsync(Quotation supplierBid)
        {
            //supplierBid.PriceValidity = _dateTimeHelper.ConvertToUtcTime(supplierBid.PriceValidity, DateTimeKind.Local);

            await _quotationRepository.InsertAsync(supplierBid);

            //if (!(await _bidRequestTrackerService.FindBidRequestTracker(supplierBid.Id, "SupplierQuotation", supplierBid.QuotationStatusId)))
            //{
            //    BidRequestTracker bidRequestTracker = new BidRequestTracker()
            //    {
            //        TargetId = supplierBid.Id,
            //        RequestId = supplierBid.RfqId,
            //        QuotationId = supplierBid.Id,
            //        TrackerType = "SupplierQuotation",
            //        StatusId = supplierBid.QuotationStatusId,
            //        UserId = (await _workContext.GetCurrentCustomerAsync()).Id,
            //        CreatedOnUtc = DateTime.UtcNow
            //    };
            //    await _bidRequestTrackerService.InsertBidRequestTrackerAsync(bidRequestTracker);
            //}
        }

        public virtual async Task UpdateQuotationAsync(Quotation supplierBid)
        {
            supplierBid.PriceValidity = _dateTimeHelper.ConvertToUtcTime(supplierBid.PriceValidity, DateTimeKind.Local);
            await _quotationRepository.UpdateAsync(supplierBid);

            //if (!(await _bidRequestTrackerService.FindBidRequestTracker(supplierBid.Id, "SupplierQuotation", supplierBid.QuotationStatusId)))
            //{
            //    BidRequestTracker bidRequestTracker = new BidRequestTracker()
            //    {
            //        TargetId = supplierBid.Id,
            //        RequestId = supplierBid.RfqId,
            //        QuotationId = supplierBid.Id,
            //        TrackerType = "SupplierQuotation",
            //        StatusId = supplierBid.QuotationStatusId,
            //        UserId = (await _workContext.GetCurrentCustomerAsync()).Id,
            //        CreatedOnUtc = DateTime.UtcNow
            //    };
            //    await _bidRequestTrackerService.InsertBidRequestTrackerAsync(bidRequestTracker);
            //}
        }

        public virtual async Task DeleteQuotationAsync(Quotation supplierBid)
        {
            await _quotationRepository.DeleteAsync(supplierBid);
        }

        public virtual async Task<Quotation> GetQuotationByIdAsync(int supplierBidId)
        {
            return await _quotationRepository.GetByIdAsync(supplierBidId);
        }

        public virtual async Task<IList<Quotation>> GetQuotationByIdsAsync(int[] supplierBidIds)
        {
            return await _quotationRepository.GetByIdsAsync(supplierBidIds);
        }

        public virtual async Task<Quotation> GetQuotationByRfqIdAndSupplierId(int supplierId = 0, int RfqId = 0)
        {
            var query = _quotationRepository.Table;

            if (supplierId > 0)
                query = query.Where(x => x.SupplierId == supplierId);
            if (RfqId > 0)
                query = query.Where(x => x.RfqId == RfqId);

            //var data = await query.FirstOrDefaultAsync(record => record.SupplierId == supplierId && record.RfqId == RfqId);
            return query.FirstOrDefault();
        }

        public virtual async Task<IList<Quotation>> GetQuotationBySupplierIdAndRfqIdAsync(int SupplierId, int RfqId, bool onlyApproved = true)
        {
            var query = _quotationRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (onlyApproved)
                query = query.Where(br => br.QuotationStatusId == (int)QuotationStatus.Approved || br.QuotationStatusId == (int)QuotationStatus.Complete);

            if (SupplierId > 0)
                query = query.Where(br => br.SupplierId == SupplierId);

            if (RfqId > 0)
                query = query.Where(br => br.RfqId == RfqId);

            return await query.ToListAsync();
        }

        //public virtual async Task<Quotation> GetQuotationByRequestIdAsync(int requestId)
        //{
        //    var query = _quotationRepository.Table;
        //    var data = await query.FirstOrDefaultAsync(record => record.RfqId == requestId);
        //    return data;
        //}

        public virtual async Task<Quotation> GetLowestQuotationByRequestIdAsync(int requestId)
        {
            var query = _quotationRepository.Table;
            query = query.Where(x => x.RfqId == requestId);

            var data = await query.FirstOrDefaultAsync(record => record.RfqId == requestId);
            return data;
        }

        public virtual async Task<IPagedList<Quotation>> GetAllQuotationAsync(
            List<int> sbIds = null,
            int RfqId = 0, int supplierId = 0, DateTime? startDateUtc = null, DateTime? endDateUtc = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, int bookerId = 0, int employeeId = 0,
            string email = null, string name = null, int industryId = 0, int categoryId = 0, bool loadOnlyExpired = false, bool orderByAscending = false, bool? getOnlyActiveBidsForApi = null)
        {
            var query = _quotationRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (getOnlyActiveBidsForApi.HasValue && getOnlyActiveBidsForApi.Value)
                query = query.Where(o => o.QuotationStatusId == (int)QuotationStatus.Verified);

            if (getOnlyActiveBidsForApi.HasValue && !getOnlyActiveBidsForApi.Value)
                query = query.Where(o => o.QuotationStatusId == (int)QuotationStatus.Cancelled || o.QuotationStatusId == (int)QuotationStatus.UnVerified || o.QuotationStatusId == (int)QuotationStatus.Expired);

            if (sbIds != null && sbIds.Any())
                query = query.Where(o => sbIds.Contains(o.QuotationStatusId));

            if (loadOnlyExpired)
                query = query.Where(br => DateTime.UtcNow > br.PriceValidity && (br.QuotationStatusId == (int)QuotationStatus.Pending || br.QuotationStatusId == (int)QuotationStatus.Verified));

            if (RfqId > 0)
                query = query.Where(bid => bid.RfqId == RfqId);

            if (bookerId > 0)
                query = query.Where(br => br.BookerId == bookerId);

            //if (bookerId < 0)
            //    query = query.Where(br => br.BookerId == 0);

            if (supplierId > 0)
                query = query.Where(bid => bid.SupplierId == supplierId);

            //filter by dates
            if (startDateUtc.HasValue)
                query = query.Where(bid => bid.CreatedOnUtc >= startDateUtc.Value);
            if (endDateUtc.HasValue)
                query = query.Where(bid => bid.CreatedOnUtc <= endDateUtc.Value);

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = from request in query
                        join c in _customerRepository.Table on request.SupplierId equals c.Id
                        where c.Email.Contains(email)
                        select request;
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = from request in query
                        join c in _customerRepository.Table on request.SupplierId equals c.Id
                        where c.FullName.Contains(name)
                        select request;
            }

            if (industryId > 0)
            {
                query = from q in query
                        join rfq in _requestForQuotationRepository.Table on q.RfqId equals rfq.Id
                        join r in _buyerRequestRepository.Table on rfq.RequestId equals r.Id
                        where r.IndustryId == industryId
                        && !r.Deleted
                        select q;
            }
            if (categoryId > 0)
            {
                query = from q in query
                        join rfq in _requestForQuotationRepository.Table on q.RfqId equals rfq.Id
                        join r in _buyerRequestRepository.Table on rfq.RequestId equals r.Id
                        where r.CategoryId == categoryId
                        && !r.Deleted
                        select q;
            }
            else
                query = query.OrderByDescending(b => b.CreatedOnUtc).ThenByDescending(s => s.Id);

            var supplierBidRequests = await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
            return supplierBidRequests;
        }

        public virtual async Task<int> GetQuotationsCountAsync(
            List<int> bsIds = null, int RfqId = 0, int supplierId = 0, DateTime? startDateUtc = null, DateTime? endDateUtc = null,
            int bookerId = 0, int employeeId = 0,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _quotationRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (bsIds != null && bsIds.Any())
                query = query.Where(o => bsIds.Contains(o.QuotationStatusId));

            if (RfqId > 0)
                query = query.Where(bid => bid.RfqId == RfqId);

            if (supplierId > 0)
                query = query.Where(bid => bid.SupplierId == supplierId);

            if (bookerId > 0)
                query = query.Where(bid => bid.BookerId == bookerId);


            //if (!showHidden)
            //{
            //    query = query.Where(b => b.Published);
            //    //query = query.Where(br => br.PriceValidity > DateTime.UtcNow);
            //}

            //filter by dates
            if (startDateUtc.HasValue)
                query = query.Where(bid => bid.CreatedOnUtc >= startDateUtc.Value);
            if (endDateUtc.HasValue)
                query = query.Where(bid => bid.CreatedOnUtc <= endDateUtc.Value);

            query = query.OrderByDescending(b => b.CreatedOnUtc);

            return (await query.ToListAsync()).Count;
        }

        public virtual async Task<DateTime?> GetLastTrackStatusByQuotationIdAsync(int quotationId, int statusId)
        {
            var data = await _bidRequestTrackerRepository.
                Table.OrderByDescending(x => x.Id).
                    FirstOrDefaultAsync(x => x.TrackerType == "SupplierQuotation" && x.StatusId == statusId && x.TargetId == quotationId);

            if (data == null)
                return null;

            return data.CreatedOnUtc;
        }

        public virtual async Task<DateTime?> GetLastUpdatedDateByStatusAsync(int statusId, int supplierId = 0, int bookerId = 0)
        {
            if (supplierId > 0)
            {
                var data = await _quotationRepository.Table.OrderByDescending(x => x.UpdatedOnUtc)
                .FirstOrDefaultAsync(x => x.QuotationStatusId == statusId && x.SupplierId == supplierId);
                if (data != null)
                    return data.UpdatedOnUtc;
            }

            if (bookerId > 0)
            {
                var data = await _quotationRepository.Table.OrderByDescending(x => x.UpdatedOnUtc)
                .FirstOrDefaultAsync(x => x.QuotationStatusId == statusId && x.BookerId == bookerId);
                if (data != null)
                    return data.UpdatedOnUtc;
            }

            return null;
        }

        public virtual async Task<Quotation> GetLastUpdatedRecordByStatusAsync(int supplierId = 0, int bookerId = 0)
        {
            if (supplierId > 0)
            {
                var data = await _quotationRepository.Table.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefaultAsync(x => x.SupplierId == supplierId);
                return data;
            }

            if (bookerId > 0)
            {
                var data = await _quotationRepository.Table.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefaultAsync(x => x.BookerId == bookerId);
                return data;
            }

            return null;
        }

        #endregion
    }
}