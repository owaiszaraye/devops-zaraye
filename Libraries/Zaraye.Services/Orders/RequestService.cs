using LinqToDB.Tools;
using Nito.AsyncEx;
using Zaraye.Core;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data;
using Zaraye.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders
{
    public partial class RequestService : IRequestService
    {
        #region Fields

        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<RequestForQuotation> _requestForQuotationRepository;
        private readonly IRepository<Quotation> _quotationRepository;
        private readonly IRepository<RequestRfqQuotationMapping> _requestRfqQuotationMappingRepository;
        private IRepository<Customer> _customerRepository;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public RequestService(
            IRepository<Request> requestRepository,
            IRepository<Customer> customerRepository,
            IRepository<RequestForQuotation> requestForQuotationRepository,
            IRepository<Quotation> quotationRepository,
            IRepository<RequestRfqQuotationMapping> requestRfqQuotationMappingRepository,
            ICustomNumberFormatter customNumberFormatter,
            IDateTimeHelper dateTimeHelper
            )
        {
            _requestRepository = requestRepository;
            _customerRepository = customerRepository;
            _requestForQuotationRepository = requestForQuotationRepository;
            _quotationRepository = quotationRepository;
            _requestRfqQuotationMappingRepository = requestRfqQuotationMappingRepository;
            _customNumberFormatter = customNumberFormatter;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region utilites
        public virtual async Task<decimal> GetRfqUsedQuantity(int requestId)
        {
            var requests = await GetAllRequestForQuotationAsync(requestId: requestId);
            return requests.Where(x => x.RfqStatusId != (int)RequestStatus.UnVerified && x.RfqStatusId != (int)RequestStatus.Cancelled).Sum(x => x.Quantity);

        }
        #endregion

        #region Methods

        #region Request

        public virtual async Task DeleteRequestAsync(Request request)
        {
            await _requestRepository.DeleteAsync(request);
        }

        public virtual async Task<Request> GetRequestByIdAsync(int id)
        {
            return await _requestRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertRequestAsync(Request request)
        {
            request.DeliveryDate = (DateTime)_dateTimeHelper.ConvertToUtcTime(request.DeliveryDate, DateTimeKind.Local);

            await _requestRepository.InsertAsync(request);
        }

        public virtual async Task UpdateRequestAsync(Request request)
        {
            request.DeliveryDate = (DateTime)_dateTimeHelper.ConvertToUtcTime(request.DeliveryDate, DateTimeKind.Local);
            await _requestRepository.UpdateAsync(request);
        }

        public virtual async Task<IPagedList<Request>> GetAllRequestAsync(int pageIndex = 0, int pageSize = int.MaxValue,
           int bookerId = 0, int categoryId = 0, int industryId = 0, List<int> bsIds = null,
           int buyerId = 0, bool? getOnlyActiveRequestsForApi = null, bool excludeRfqForApp = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null,
            string searchBuyerEmail = null, string searchBuyerName = null, bool getOnlyTotalCount = false, int requestTypeId = (int)RequestTypeEnum.External, string source = "", int pocId = 0, int createdbyId = 0)
        {
            var query = from ut in _requestRepository.Table
                        where !ut.Deleted && ut.RequestTypeId == requestTypeId
                        select ut;

            if (bookerId > 0)
                query = query.Where(r => r.BookerId == bookerId);

            if (getOnlyActiveRequestsForApi.HasValue && getOnlyActiveRequestsForApi.Value)
                query = query.Where(o => o.RequestStatusId == (int)RequestStatus.Pending || o.RequestStatusId == (int)RequestStatus.Verified);

            if (getOnlyActiveRequestsForApi.HasValue && !getOnlyActiveRequestsForApi.Value)
                query = query.Where(o => o.RequestStatusId == (int)RequestStatus.Cancelled || o.RequestStatusId == (int)RequestStatus.UnVerified || o.RequestStatusId == (int)RequestStatus.Expired);

            if (categoryId > 0)
                query = query.Where(r => r.CategoryId == categoryId);

            if (industryId > 0)
                query = query.Where(r => r.IndustryId == industryId);

            if (buyerId > 0)
                query = query.Where(r => r.BuyerId == buyerId);

            if (bsIds != null && bsIds.Any())
                query = query.Where(r => bsIds.Contains(r.RequestStatusId));

            if (pocId > 0)
                query = query.Where(r => r.PocId == pocId);
            if (createdbyId > 0)
                query = query.Where(r => r.CreatedById == createdbyId);

            if (startDateUtc.HasValue)
                query = query.Where(r => startDateUtc.Value <= r.CreatedOnUtc);

            if (endDateUtc.HasValue)
                query = query.Where(r => endDateUtc.Value >= r.CreatedOnUtc);

            if (excludeRfqForApp)
            {
                var requestIds = new List<int>();
                foreach (var r in query.ToList())
                {
                    var totalUsedQuantity = await GetRfqUsedQuantity(r.Id);
                    var totalRfqRemainingQty = r.Quantity - totalUsedQuantity;

                    if (totalRfqRemainingQty == 0)
                        requestIds.Add(r.Id);
                }
                query = query.Where(x => !requestIds.Contains(x.Id));
            }

            //buyer email filtering
            if (!string.IsNullOrWhiteSpace(searchBuyerEmail))
            {
                query = from br in query
                        join c in _customerRepository.Table on br.BuyerId equals c.Id
                        where c.Email.Contains(searchBuyerEmail)
                        select br;
            }

            //buyer name filtering
            if (!string.IsNullOrWhiteSpace(searchBuyerName))
            {
                query = from br in query
                        join c in _customerRepository.Table on br.BuyerId equals c.Id
                        where c.FullName.Contains(searchBuyerName)
                        select br;
            }

            //source filtering
            if (!string.IsNullOrWhiteSpace(source) && source != "0")
                query = query.Where(r => r.Source.Contains(source));

            var request = await query.OrderByDescending(x => x.Id).ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);

            //paging
            return request;
        }


        public virtual async Task<Request> GetLastUpdatedRecordByStatusAsync(int buyerId = 0, int bookerId = 0)
        {
            if (buyerId > 0)
            {
                var data = await _requestRepository.Table.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefaultAsync(x => x.BuyerId == buyerId);
                return data;
            }

            if (bookerId > 0)
            {
                var data = await _requestRepository.Table.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefaultAsync(x => x.BookerId == bookerId);
                return data;
            }

            return null;
        }

        public virtual async Task<DateTime?> GetLastUpdatedDateByStatusAsync(int statusId, int buyerId = 0, int bookerId = 0)
        {
            if (buyerId > 0)
            {
                var data = await _requestRepository.Table.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefaultAsync(x => x.RequestStatusId == statusId && x.BuyerId == buyerId);
                if (data != null)
                    return data.UpdatedOnUtc;
            }

            if (bookerId > 0)
            {
                var data = await _requestRepository.Table.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefaultAsync(x => x.RequestStatusId == statusId && x.BookerId == bookerId);
                if (data != null)
                    return data.UpdatedOnUtc;
            }

            return null;
        }

        public virtual async Task<int> GetQuotationCountByRequestIdAsync(int requestId)
        {
            return await (from r in _requestRepository.Table
                          join rfq in _requestForQuotationRepository.Table on r.Id equals rfq.RequestId
                          join q in _quotationRepository.Table on rfq.Id equals q.RfqId
                          where !rfq.Deleted && q.QuotationStatusId == (int)QuotationStatus.Verified &&
                          r.Id == requestId
                          select r).CountAsync();
        }

        public virtual async Task<IList<Quotation>> GetQuotationsByRequestIdAsync(int requestId)
        {
            return await (from r in _requestRepository.Table
                          join rfq in _requestForQuotationRepository.Table on r.Id equals rfq.RequestId
                          join q in _quotationRepository.Table on rfq.Id equals q.RfqId
                          where !rfq.Deleted && q.QuotationStatusId == (int)QuotationStatus.Verified &&
                          r.Id == requestId
                          select q).ToListAsync();
        }

        #endregion

        #region Request For Quotation

        public virtual async Task<RequestForQuotation> GetRequestForQuotationByRequestIdAsync(int requestId)
        {
            if (requestId == 0)
                return null;

            var query = from p in _requestForQuotationRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.RequestId == requestId
                        select p;
            var product = await query.FirstOrDefaultAsync();

            return product;
        }

        public virtual async Task DeleteRequestForQuotationAsync(RequestForQuotation requestForQuotation)
        {
            await _requestForQuotationRepository.DeleteAsync(requestForQuotation);
        }

        public virtual async Task<RequestForQuotation> GetRequestForQuotationByIdAsync(int id)
        {
            return await _requestForQuotationRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertRequestForQuotationAsync(RequestForQuotation requestForQuotation)
        {
            await _requestForQuotationRepository.InsertAsync(requestForQuotation);
        }

        public virtual async Task UpdateRequestForQuotationAsync(RequestForQuotation requestForQuotation)
        {
            await _requestForQuotationRepository.UpdateAsync(requestForQuotation);
        }

        public virtual async Task<IPagedList<RequestForQuotation>> GetAllRequestForQuotationAsync(int pageIndex = 0, int pageSize = int.MaxValue,
            int requestId = 0, int rfqStatusId = 0, int categoryId = 0, int industryId = 0, int productId = 0, int bookerId = 0, List<int> rfqsIds = null,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null)
        {
            var query = from ut in _requestForQuotationRepository.Table where !ut.Deleted select ut;

            if (requestId > 0)
                query = query.Where(r => r.RequestId == requestId);

            if (rfqStatusId > 0)
                query = query.Where(r => r.RfqStatusId == rfqStatusId);

            if (categoryId > 0)
            {
                query = from rfq in query
                        join r in _requestRepository.Table on rfq.RequestId equals r.Id
                        where r.CategoryId == categoryId
                        select rfq;
            }

            if (industryId > 0)
            {
                query = from rfq in query
                        join r in _requestRepository.Table on rfq.RequestId equals r.Id
                        where r.IndustryId == industryId
                        select rfq;
            }

            if (productId > 0)
            {
                query = from rfq in query
                        join r in _requestRepository.Table on rfq.RequestId equals r.Id
                        where r.ProductId == productId
                        select rfq;
            }

            if (rfqsIds != null && rfqsIds.Any())
                query = query.Where(r => rfqsIds.Contains(r.RfqStatusId));

            if (bookerId > 0)
                query = query.Where(r => r.BookerId == bookerId);

            if (startDateUtc.HasValue)
                query = query.Where(r => startDateUtc.Value <= r.CreatedOnUtc);

            if (endDateUtc.HasValue)
                query = query.Where(r => endDateUtc.Value >= r.CreatedOnUtc);

            var requestForQuotation = await query.OrderByDescending(x => x.Id).ToListAsync();

            //paging
            return new PagedList<RequestForQuotation>(requestForQuotation, pageIndex, pageSize);
        }

        #endregion

        #region Request Rfq Quotation Mapping

        public virtual async Task DeleteRequestRfqQuotationMappingAsync(RequestRfqQuotationMapping requestRfqQuotationMapping)
        {
            await _requestRfqQuotationMappingRepository.DeleteAsync(requestRfqQuotationMapping);
        }

        public virtual async Task<RequestRfqQuotationMapping> GetRequestRfqQuotationMappingByIdAsync(int id)
        {
            return await _requestRfqQuotationMappingRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertRequestRfqQuotationMappingAsync(RequestRfqQuotationMapping requestRfqQuotationMapping)
        {
            await _requestRfqQuotationMappingRepository.InsertAsync(requestRfqQuotationMapping);
        }

        public virtual async Task UpdateRequestRfqQuotationMappingAsync(RequestRfqQuotationMapping requestRfqQuotationMapping)
        {
            await _requestRfqQuotationMappingRepository.UpdateAsync(requestRfqQuotationMapping);
        }

        public virtual async Task<IPagedList<RequestRfqQuotationMapping>> GetAllRequestRfqQuotationMappingAsync(int orderId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from ut in _requestRfqQuotationMappingRepository.Table
                        select ut;

            if (orderId > 0)
                query = query.Where(r => r.OrderId == orderId);

            var requestRfqQuotationMapping = await query.ToListAsync();

            //paging
            return new PagedList<RequestRfqQuotationMapping>(requestRfqQuotationMapping, pageIndex, pageSize);
        }

        public virtual async Task<RequestRfqQuotationMapping> GetRequestRfqQuotationMappingByOrderAsync(int orderId)
        {
            return await (from inv in _requestRfqQuotationMappingRepository.Table
                          where inv.OrderId == orderId
                          select inv).FirstOrDefaultAsync();
        }

        #endregion

        #endregion
    }
}
