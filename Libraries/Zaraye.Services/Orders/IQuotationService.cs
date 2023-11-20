using Zaraye.Core;
using Zaraye.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders
{
    public partial interface IQuotationService
    {
        #region Methods

        Task InsertQuotationAsync(Quotation quotation);

        Task UpdateQuotationAsync(Quotation quotation);

        Task DeleteQuotationAsync(Quotation quotation);
        Task<Quotation> GetQuotationByIdAsync(int quotationId);

        Task<IList<Quotation>> GetQuotationByIdsAsync(int[] quotationIds);
        Task<Quotation> GetQuotationByRfqIdAndSupplierId(int supplierId = 0, int RfqId = 0);
        Task<IList<Quotation>> GetQuotationBySupplierIdAndRfqIdAsync(int SupplierId, int RfqId, bool onlyApproved = true);

        Task<Quotation> GetLowestQuotationByRequestIdAsync(int requestId);

        Task<IPagedList<Quotation>> GetAllQuotationAsync(
           List<int> sbIds = null,
           int RfqId = 0, int supplierId = 0, DateTime? startDateUtc = null, DateTime? endDateUtc = null,
           bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, int bookerId = 0, int employeeId = 0,
           string email = null, string name = null, int industryId = 0, int categoryId = 0, bool loadOnlyExpired = false, bool orderByAscending = false, bool? getOnlyActiveBidsForApi = null);

        Task<int> GetQuotationsCountAsync(
           List<int> bsIds = null, int RfqId = 0, int supplierId = 0, DateTime? startDateUtc = null, DateTime? endDateUtc = null,
           int bookerId = 0, int employeeId = 0,
           bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
        //Task<IPagedList<BuyerRequest>> GetAllRequestsBySupplierIdAsync(int supplierId, int requestId = 0, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<DateTime?> GetLastTrackStatusByQuotationIdAsync(int quotationId, int statusId);
        Task<DateTime?> GetLastUpdatedDateByStatusAsync(int statusId, int supplierId = 0, int bookerId = 0);
        Task<Quotation> GetLastUpdatedRecordByStatusAsync(int supplierId = 0, int bookerId = 0);

        #endregion
    }
}