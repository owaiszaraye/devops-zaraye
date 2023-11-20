using Zaraye.Models.Api.V4.Buyer;

namespace Zaraye.Services.Buyer
{
    public interface IBuyerService
    {
        Task<object> GetDashboardData();
        Task<object> GetInfo();
        Task<object> UpdateInfo(BuyerInfoApiModel model);
        Task<string> UpdateAddress(BuyerInfoAddressApiModel model);
        Task<IList<object>> GetBuyerRequestHistory(bool active = false);
        Task<object> GetBuyerRequest(int requestId);
        Task<object> AddBuyerRequest(BuyerRequestApiModel model);
        Task<object> EditBuyerRequest(int requestId, BuyerRequestApiModel model);
        Task<IList<object>> BuyerRequestQuotationHistory(int requestId, List<int> statusIds = null);
        Task<object> BuyerRequestQuotationApproved(BuyerRequestBidApproveApiModel model);
        Task<IList<object>> BuyerOrderList(bool active);
        Task<object> BuyerOrderDetail(int orderId);
        Task<object> BuyerOrderSummary(int orderId);
        Task<object> BuyerRequestStatusSummary();
        Task<object> SupportAgentInfo();
        Task<object> BuyerLedgerDetail(DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex);
        Task<object> Buyer_BuyerContactUploadSignature(BuyerContractUploadSignatureModel model);
        Task ApplyForCreditCustomer(ApplyCreditCustomerModel model);
        Task<object> GetApplyForCreditCustomer();
        Task<string> CreditApplication(CreditApplicationModel model);
    }
}
