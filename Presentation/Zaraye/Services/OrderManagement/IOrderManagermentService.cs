using Microsoft.AspNetCore.Mvc;
using Zaraye.Models.Api.V4.OrderManagement;
using Zaraye.Models.Api.V4.Security;
using static Zaraye.Models.Api.V4.OrderManagement.OrderManagementApiModel;

namespace Zaraye.Services.OrderManagement
{
    public interface IOrderManagermentService
    {
        Task<object> Info();
        Task<string> InfoAsync(BuyerInfoModel model);
        Task<object> AllOpsAgents();
        Task<object> GetAllBankDetailsBySupplier(int supplierId);
        Task<object> AllFinanceAgents();
        Task<string> BuyerRegistration(AccountApiModel.BookerBuyerRegisterApiModel model);
        Task<object> SearchBuyers(string name = "");
        Task<object> AddBuyerRequest(BuyerRequestModel model);
        Task<string> RejectRequest(RejectModel model);
        Task<object> AddRequestForQuotation(RequestForQuotationModel model);
        Task<object> RequestHistory(int pageIndex = 0, int pageSize = 10);
        Task<object> GetBuyerRequest(int requestId);
        Task<object> GetAllRequests(int industryId, int pageIndex = 0, int pageSize = 10);
        Task<object> GetPurchaseOrderDetailForDeliveryRequest(int expectedShipmentId);
        Task<object> GetSaleOrderDetailForDeliveryRequest(int expectedShipmentId);
        Task<object> AddShipmentRequest(OrderDeliveryRequestModel model);
        Task<object> SaleOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment");

        Task<object> CheckDirectSaleOrderExist(int requestId);
        Task<object> DirectSaleOrderProcess(int requestId);
        Task<object> GetSaleOrderFormLoad(int requestId);
        Task<object> SaleOrderInfo(DirectOrderApiModel.DirectOrderInfoModel model);
        Task<object> SaleOrderBusinessModelFormJson(int requestId);
        Task<object> SaleOrderBusinessModelFormCalculation(BusinessModelApiModel model);
        Task<object> DirectSaleOrder(int requestId);
        Task<object> SaleOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10);
        Task<object> SaleOrderDetail(int orderId);
        Task<object> BuyerOrderSummary(int orderId);


        Task<object> CogsInventoryList(int requestId);
        Task<object> GetInventoriesByRequest(int requestId);
        Task<object> AddCogsInventoryTagging(DirectOrderApiModel.DirectCogsInventoryTaggingModel model);
        Task<object> DeleteCogsInventoryTagging(int id);

        Task<object> TempOrder_AddExpectedShipment(int tempOrderId);
        Task<object> TempOrder_UpdateExpectedShipment(DirectOrderApiModel.DirectOrderDeliveryScheduleModel model);
        Task<object> TempOrder_DeleteExpectedDeliverySchedule(int expectedShipmentId);

        Task<object> SellerRegistration(AccountApiModel.BookerSellerRegisterApiModel model);
        Task<object> SearchSuppliers(string name = "");
        Task<object> SearchSuppliersByProductId(int productId);
        Task<object> GetRequestForQuotationHistory();
        Task<object> QuotationHistory();
        Task<object> MarketDataBySupplier(int industryId, int categoryId = 0, int productId = 0, DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetRequestForQuotation(int requestForQuotationId);
        Task<object> AddQuotationMultiple(int requestForQuotationId, List<RFQQuotationsModel> model);
        Task<object> GetAllQuotationsByRFQId(int requestForQuotationId);
        Task<object> RejectRequestForQuotation(RejectModel model);
        Task<object> GetSupplierContract(int orderId, int supplierId);
        Task<object> GetOrderDetailForPickupSchedule(int expectedShipmentId);
        Task<object> AddPickupRequest(OrderDeliveryRequestModel model);
        Task<object> PickupRequestDeatil(int expectedShipmentId);
        Task<object> AddPurchaseOrderShipmentRequest([FromBody] OrderDeliveryRequestModel model);
        Task<object> PurchaseOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment");


        Task<object> CheckDirectPurchaseOrderExist(int requestForQuotationId);
        Task<object> DirecPurchaseOrderProcess(int requestForQuotationId, List<RFQQuotationsModel> model);
        Task<object> GeneratePurchaseDirectOrder(int requestForQuotationId, List<RFQQuotationsModel> model);
        Task<object> GetAllDirectOrderByRequestForQuotationId(int requestForQuotationId);
        Task<object> DirectPurchaseOrderInfo(int directOrderId, DirectOrderApiModel.DirectOrderInfoModel model);
        Task<object> PurchaseOrderBusinessModelFormJson(int requestForQuotationId);
        Task<object> PurchaseOrderBusinessModelFormCalculation(BusinessModelApiModel model);
        Task<object> DirectPurchaseOrderPlaced(int requestForQuotationId);
        Task<object> PurchaseOrderDetail(int orderId);
        Task<object> PurchaseOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10);
        Task<object> SupplierOrderSummary(int orderId);


        Task<object> SaleOrderAcceptShipmentRequest(int shipmentRequestId);
        Task<object> PurchaseOrderAcceptShipmentRequest(int shipmentRequestId);
        Task<object> ShipmentRequestReject(RejectDeliveryRequest model);
        Task<object> ReAssignAgent(ReAssignAgent model);
        Task<object> OrderDeliveryRequestMarkAsIncomplete(int deliveryRequestId);

        Task<object> GetAllTickets(string status = "", string orderby = "");


        Task<object> BuyerContactUploadSignature(BuyerContactUploadSignatureModel model);
        Task<object> SupplierContactUploadSignature(BuyerContactUploadSignatureModel model);

        Task<object> GetExpectedShipmentsByOrderId(int orderId);


        Task<object> GetAllShipments(int ShipmentTypeId = 0);
        Task<object> SaleOrderMarkAsShipped(MaskAsShippedModel model);
        Task<object> SaleOrderMarkAsDelivered(MaskAsDeliveredModel model);
        Task<object> PurchaseOrderMarkAsShipped(MaskAsShippedModel model);
        Task<object> PurchaseOrderMarkAsDelivered(MaskAsDeliveredModel model);
        Task<object> GetAllTransporters();
        Task<object> GetVehiclesByTransporterId(int transporterId);



    }
}
