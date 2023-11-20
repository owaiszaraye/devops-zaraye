using Zaraye.Core;
using Zaraye.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders
{
    public partial interface IRequestService
    {
        Task<decimal> GetRfqUsedQuantity(int requestId);

        #region Request

        Task DeleteRequestAsync(Request request);

        Task<Request> GetRequestByIdAsync(int id);

        Task InsertRequestAsync(Request request);

        Task UpdateRequestAsync(Request request);

        Task<IPagedList<Request>> GetAllRequestAsync(int pageIndex = 0, int pageSize = int.MaxValue,
           int bookerId = 0, int categoryId = 0, int industryId = 0, List<int> bsIds = null,
           int buyerId = 0, bool? getOnlyActiveRequestsForApi = null, bool excludeRfqForApp = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null,
            string searchBuyerEmail = null, string searchBuyerName = null, bool getOnlyTotalCount = false, int requestTypeId = (int)RequestTypeEnum.External,string source="", int pocId = 0, int createdbyId = 0);

        Task<Request> GetLastUpdatedRecordByStatusAsync(int buyerId = 0, int bookerId = 0);

        Task<DateTime?> GetLastUpdatedDateByStatusAsync(int statusId, int buyerId = 0, int bookerId = 0);

        Task<int> GetQuotationCountByRequestIdAsync(int requestId);

        Task<IList<Quotation>> GetQuotationsByRequestIdAsync(int requestId);
        #endregion

        #region RequestForQuotation

        Task<RequestForQuotation> GetRequestForQuotationByRequestIdAsync(int requestId);

        Task DeleteRequestForQuotationAsync(RequestForQuotation requestForQuotation);

        Task<RequestForQuotation> GetRequestForQuotationByIdAsync(int id);

        Task InsertRequestForQuotationAsync(RequestForQuotation requestForQuotation);

        Task UpdateRequestForQuotationAsync(RequestForQuotation requestForQuotation);

        Task<IPagedList<RequestForQuotation>> GetAllRequestForQuotationAsync(int pageIndex = 0, int pageSize = int.MaxValue,
             int requestId = 0, int rfqStatusId = 0, int categoryId = 0, int industryId = 0, int productId = 0, int bookerId = 0, List<int> rfqsIds = null,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null);

        #endregion

        #region RequestRfqQuotationMapping

        Task DeleteRequestRfqQuotationMappingAsync(RequestRfqQuotationMapping requestRfqQuotationMapping);

        Task<RequestRfqQuotationMapping> GetRequestRfqQuotationMappingByIdAsync(int id);

        Task InsertRequestRfqQuotationMappingAsync(RequestRfqQuotationMapping requestRfqQuotationMapping);

        Task UpdateRequestRfqQuotationMappingAsync(RequestRfqQuotationMapping requestRfqQuotationMapping);

        Task<IPagedList<RequestRfqQuotationMapping>> GetAllRequestRfqQuotationMappingAsync(int orderId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<RequestRfqQuotationMapping> GetRequestRfqQuotationMappingByOrderAsync(int orderId);

        #endregion
    }
}
