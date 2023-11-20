using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaraye.Services.Logging;
using Zaraye.Models.Api.V4.OrderManagement;
using Zaraye.Models.Api.V4.Security;
using Zaraye.Services.OrderManagement;

namespace Zaraye.Controllers.V4.OrderManagement
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/order-management")]
    [AuthorizeApi]
    public class OrderManagementController : BaseApiController
    {
        #region Fields
        private readonly IOrderManagermentService _orderManagermentService;
        private readonly IAppLoggerService _appLoggerService;

        #endregion

        #region Ctor

        public OrderManagementController(IOrderManagermentService orderManagermentService, IAppLoggerService appLoggerService)
        {
            _orderManagermentService = orderManagermentService;
            _appLoggerService = appLoggerService;
        }

        #endregion

        #region Common Api

        [HttpGet("user-info")]
        public async Task<IActionResult> OrderManagement_Info()
        {

            try
            {
                var data = await _orderManagermentService.Info();
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("user-info")]
        public async Task<IActionResult> OrderManagement_Info([FromBody] OrderManagementApiModel.BuyerInfoModel model)
        {
            try
            {
                var data = await _orderManagermentService.InfoAsync(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-live-ops-agents")]
        public async Task<IActionResult> OrderManagement_AllOpsAgents()
        {
            try
            {
                var data = await _orderManagermentService.AllOpsAgents();
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("bank-details-by-supplier/{supplierId}")]
        public async Task<IActionResult> OrderManagement_GetAllBankDetailsBySupplier(int supplierId)
        {
            try
            {
                var data = await _orderManagermentService.GetAllBankDetailsBySupplier(supplierId);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-finance-agents")]
        public async Task<IActionResult> OrderManagement_AllFinanceAgents()
        {
            try
            {
                var data = await _orderManagermentService.AllFinanceAgents();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Booker

        [AllowAnonymous]
        [HttpPost("buyer-registration")]
        public async Task<IActionResult> OrderManagement_BuyerRegistration([FromBody] AccountApiModel.BookerBuyerRegisterApiModel model)
        {
            try
            {
                var data = await _orderManagermentService.BuyerRegistration(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("search-buyers/{name}")]
        public async Task<IActionResult> OrderManagement_SearchBuyers(string name = "")
        {
            try
            {
                var data = await _orderManagermentService.SearchBuyers(name);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("buyer-request-add")]
        public async Task<IActionResult> OrderManagement_AddBuyerRequest([FromBody] OrderManagementApiModel.BuyerRequestModel model)
        {
            try
            {
                var data = await _orderManagermentService.AddBuyerRequest(model);

                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("reject-request")]
        public async Task<IActionResult> OrderManagement_RejectRequest([FromBody] OrderManagementApiModel.RejectModel model)
        {
            try
            {
                var data = await _orderManagermentService.RejectRequest(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("request-for-quotation-add")]
        public async Task<IActionResult> OrderManagement_AddRequestForQuotation([FromBody] OrderManagementApiModel.RequestForQuotationModel model)
        {
            try
            {
                var data = await _orderManagermentService.AddRequestForQuotation(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-request-history")]
        public async Task<IActionResult> OrderManagement_RequestHistory(int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                var data = await _orderManagermentService.RequestHistory(pageIndex, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-request/{requestId}")]
        public async Task<IActionResult> OrderManagement_GetBuyerRequest(int requestId)
        {
            try
            {
                var data = await _orderManagermentService.GetBuyerRequest(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-requests")]
        public async Task<IActionResult> OrderManagement_GetAllRequests(int industryId, int pageIndex = 0, int pageSize = 10)
        {

            try
            {
                var data = await _orderManagermentService.GetAllRequests(industryId, pageIndex, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-purchase-order-detail-for-shipment-request/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_GetPurchaseOrderDetailForDeliveryRequest(int expectedShipmentId)
        {
            try
            {
                var data = await _orderManagermentService.GetPurchaseOrderDetailForDeliveryRequest(expectedShipmentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpGet("get-sale-order-detail-for-shipment-request/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_GetSaleOrderDetailForDeliveryRequest(int expectedShipmentId)
        {
            try
            {
                var data = await _orderManagermentService.GetSaleOrderDetailForDeliveryRequest(expectedShipmentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpPost("add-shipment-request")]
        public async Task<IActionResult> OrderManagement_AddShipmentRequest([FromBody] OrderManagementApiModel.OrderDeliveryRequestModel model)
        {
            try
            {
                var data = await _orderManagermentService.AddShipmentRequest(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-delivery-request-detail/{expectedShipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_SaleOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment")
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderDeliveryRequestDeatil(expectedShipmentRequestId, type);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Direct Sale Order

        [HttpGet("check-direct-sale-order/{requestId}")]
        public async Task<IActionResult> OrderManagement_CheckDirectSaleOrderExist(int requestId)
        {

            try
            {
                var data = await _orderManagermentService.CheckDirectSaleOrderExist(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("direct-new-sale-order-process/{requestId}")]
        public async Task<IActionResult> OrderManagement_DirectSaleOrderProcess(int requestId)
        {
            try
            {
                var data = await _orderManagermentService.DirectSaleOrderProcess(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-form-load/{requestId}")]
        public async Task<IActionResult> OrderManagement_GetSaleOrderFormLoad(int requestId)
        {
            try
            {
                var data = await _orderManagermentService.GetSaleOrderFormLoad(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("sale-order-info")]
        public async Task<IActionResult> DirectOrder_SaleOrderInfo([FromBody] DirectOrderApiModel.DirectOrderInfoModel model)
        {

            try
            {
                var data = await _orderManagermentService.SaleOrderInfo(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-business-model-form-Json")]
        public async Task<IActionResult> OrderManagement_SaleOrderBusinessModelFormJson(int requestId)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderBusinessModelFormJson(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("sale-order-business-model-form-calculation")]
        public async Task<IActionResult> OrderManagement_SaleOrderBusinessModelFormCalculation(BusinessModelApiModel model)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderBusinessModelFormCalculation(model);
                return Ok(data);
            }

            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("direct-sale-order-placed/{requestId}")]
        public async Task<IActionResult> OrderManagement_DirectSaleOrder(int requestId)
        {
            try
            {
                var data = _orderManagermentService.DirectSaleOrder(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-list")]
        public async Task<IActionResult> OrderManagement_SaleOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderList(showActiveOrders, pageIndex, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-detail/{orderId}")]
        public async Task<IActionResult> OrderManagement_SaleOrderDetail(int orderId)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderDetail(orderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-order-summary/{orderId}")]
        public async Task<IActionResult> OrderManagement_BuyerOrderSummary(int orderId)
        {

            try
            {
                var data = await _orderManagermentService.BuyerOrderSummary(orderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Cost Of Goods Selling

        [HttpGet("cogs-inventory-list/{requestId}")]
        public async Task<IActionResult> OrderManagement_CogsInventoryList(int requestId)
        {
            try
            {
                var data = await _orderManagermentService.CogsInventoryList(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-inventories-by-request/{requestId}")]
        public async Task<IActionResult> OrderManagement_GetInventoriesByRequest(int requestId)
        {
            try
            {
                var data = await _orderManagermentService.GetInventoriesByRequest(requestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-cogs-inventory-tagging")]
        public async Task<IActionResult> OrderManagement_AddCogsInventoryTagging([FromBody] DirectOrderApiModel.DirectCogsInventoryTaggingModel model)
        {
            try
            {
                var data = await _orderManagermentService.AddCogsInventoryTagging(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("delete-cogs-inventory-tagging/{id}")]
        public async Task<IActionResult> OrderManagement_DeleteCogsInventoryTagging(int id)
        {
            try
            {
                var data = await _orderManagermentService.DeleteCogsInventoryTagging(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Temp Order Expected shipments

        [HttpGet("add-expected-shipment/{tempOrderId}")]
        public async Task<IActionResult> OrderManagement_TempOrder_AddExpectedShipment(int tempOrderId)
        {
            try
            {
                var data = await _orderManagermentService.TempOrder_AddExpectedShipment(tempOrderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("update-expected-shipment")]
        public async Task<IActionResult> OrderManagement_TempOrder_UpdateExpectedShipment([FromBody] DirectOrderApiModel.DirectOrderDeliveryScheduleModel model)
        {
            try
            {
                var data = await _orderManagermentService.TempOrder_UpdateExpectedShipment(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("delete-expected-shipment/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_TempOrder_DeleteExpectedDeliverySchedule(int expectedShipmentId)
        {
            try
            {
                var data = await _orderManagermentService.TempOrder_DeleteExpectedDeliverySchedule(expectedShipmentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Supplier Booker

        [AllowAnonymous]
        [HttpPost("seller-registration")]
        public async Task<IActionResult> OrderManagement_SellerRegistration([FromBody] AccountApiModel.BookerSellerRegisterApiModel model)
        {
            try
            {
                var data = await _orderManagermentService.SellerRegistration(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search-suppliers/{name}")]
        public async Task<IActionResult> OrderManagement_SearchSuppliers(string name = "")
        {
            try
            {
                var data = await _orderManagermentService.SearchSuppliers(name);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search-suppliers-by-product/{productId}")]
        public async Task<IActionResult> OrderManagement_SearchSuppliersByProductId(int productId)
        {
            try
            {
                var data = await _orderManagermentService.SearchSuppliersByProductId(productId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("request-for-quotation-history")]
        public async Task<IActionResult> OrderManagement_GetRequestForQuotationHistory()
        {
            try
            {
                var data = await _orderManagermentService.GetRequestForQuotationHistory();
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("quotation-history")]
        public async Task<IActionResult> OrderManagement_QuotationHistory()
        {
            try
            {
                var data = await _orderManagermentService.QuotationHistory();
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("seller-market-data")]
        public async Task<IActionResult> OrderManagement_MarketDataBySupplier(int industryId, int categoryId = 0, int productId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var data = await _orderManagermentService.MarketDataBySupplier(industryId, categoryId, productId, startDate, endDate);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("request-for-quotation/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_GetRequestForQuotation(int requestForQuotationId)
        {
            try
            {
                var data = await _orderManagermentService.GetRequestForQuotation(requestForQuotationId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("quotation-add-multiple/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_AddQuotationMultiple(int requestForQuotationId, [FromBody] List<OrderManagementApiModel.RFQQuotationsModel> model)
        {
            try
            {
                var data = await _orderManagermentService.AddQuotationMultiple(requestForQuotationId, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-quotations-by-rfq-Id/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_GetAllQuotationsByRFQId(int requestForQuotationId)
        {
            try
            {
                var data = await _orderManagermentService.GetAllQuotationsByRFQId(requestForQuotationId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("request-for-quotation-reject")]
        public async Task<IActionResult> OrderManagement_RejectRequestForQuotation([FromBody] OrderManagementApiModel.RejectModel model)
        {
            try
            {
                var data = await _orderManagermentService.RejectRequestForQuotation(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("get-supplier-contract/{orderId}/{supplierId}")]
        public async Task<IActionResult> OrderManagement_GetSupplierContract(int orderId, int supplierId)
        {
            try
            {
                var data = await _orderManagermentService.GetSupplierContract(orderId, supplierId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-order-detail-for-pickup-shedules/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_GetOrderDetailForPickupSchedule(int expectedShipmentId)
        {
            try
            {
                var data = await _orderManagermentService.GetOrderDetailForPickupSchedule(expectedShipmentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpPost("add-pickup-request")]
        public async Task<IActionResult> OrderManagement_AddPickupRequest([FromBody] OrderManagementApiModel.OrderDeliveryRequestModel model)
        {
            try
            {
                var data = await _orderManagermentService.AddPickupRequest(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("pickup-request-detail/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_PickupRequestDeatil(int expectedShipmentId)
        {
            try
            {
                var data = await _orderManagermentService.PickupRequestDeatil(expectedShipmentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-purchase-order-shipment-request")]
        public async Task<IActionResult> OrderManagement_AddPurchaseOrderShipmentRequest([FromBody] OrderManagementApiModel.OrderDeliveryRequestModel model)
        {
            try
            {
                var data = await _orderManagermentService.AddPurchaseOrderShipmentRequest(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-delivery-request-detail/{expectedShipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment")
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderDeliveryRequestDeatil(expectedShipmentRequestId, type);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Direct Purchase Order

        [HttpGet("check-direct-purchase-order/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_CheckDirectPurchaseOrderExist(int requestForQuotationId)
        {

            try
            {
                var data = await _orderManagermentService.CheckDirectPurchaseOrderExist(requestForQuotationId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [HttpPost("direct-new-purchase-order-process/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_DirecPurchaseOrderProcess(int requestForQuotationId, [FromBody] List<OrderManagementApiModel.RFQQuotationsModel> model)
        {

            try
            {
                var data = await _orderManagermentService.DirecPurchaseOrderProcess(requestForQuotationId, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("generate-po")]
        public async Task<IActionResult> OrderManagement_GeneratePurchaseDirectOrder(int requestForQuotationId, [FromBody] List<OrderManagementApiModel.RFQQuotationsModel> model)
        {
            try
            {
                var data = await _orderManagermentService.GeneratePurchaseDirectOrder(requestForQuotationId, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-direct-order-by-request-for-quotation/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_GetAllDirectOrderByRequestForQuotationId(int requestForQuotationId)
        {
            try
            {
                var data = await _orderManagermentService.GetAllDirectOrderByRequestForQuotationId(requestForQuotationId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("direct-purchase-order-detail-info/{directOrderId}")]
        public async Task<IActionResult> OrderManagement_DirectPurchaseOrderInfo(int directOrderId, [FromBody] DirectOrderApiModel.DirectOrderInfoModel model)
        {
            try
            {
                var data = await _orderManagermentService.DirectPurchaseOrderInfo(directOrderId, model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-business-model-form-Json/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderBusinessModelFormJson(int requestForQuotationId)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderBusinessModelFormJson(requestForQuotationId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("purchase-order-business-model-form-calculation")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderBusinessModelFormCalculation(BusinessModelApiModel model)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderBusinessModelFormCalculation(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("direct-purchase-order-placed/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_DirectPurchaseOrderPlaced(int requestForQuotationId)
        {
            try
            {
                var data = await _orderManagermentService.DirectPurchaseOrderPlaced(requestForQuotationId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-detail/{orderId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderDetail(int orderId)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderDetail(orderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-list")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderList(showActiveOrders, pageIndex, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("supplier-order-summary/{orderId}")]
        public async Task<IActionResult> OrderManagement_SupplierOrderSummary(int orderId)
        {
            try
            {
                var data = await _orderManagermentService.SupplierOrderSummary(orderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Order Delivery Request

        [HttpPost("Sale-order-accept-shipment-request/{shipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_SaleOrderAcceptShipmentRequest(int shipmentRequestId)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderAcceptShipmentRequest(shipmentRequestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Purchase-order-accept-shipment-request/{shipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderAcceptShipmentRequest(int shipmentRequestId)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderAcceptShipmentRequest(shipmentRequestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("shipment-request-reject")]
        public async Task<IActionResult> OrderManagement_ShipmentRequestReject([FromBody] OrderManagementApiModel.RejectDeliveryRequest model)
        {
            try
            {
                var data = await _orderManagermentService.ShipmentRequestReject(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("re-assign-agent")]
        public async Task<IActionResult> OrderManagement_ReAssignAgent([FromBody] OrderManagementApiModel.ReAssignAgent model)
        {
            try
            {
                var data = await _orderManagermentService.ReAssignAgent(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //Dont have permission
        [HttpPost("order-delivery-request-mark-as-incomplete/{deliveryRequestId}")]
        public async Task<IActionResult> OrderManagement_OrderDeliveryRequestMarkAsIncomplete(int deliveryRequestId)
        {
            try
            {
                var data = await _orderManagermentService.OrderDeliveryRequestMarkAsIncomplete(deliveryRequestId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Payment orders

        [HttpGet("all-tickets")]
        public async Task<IActionResult> OrderManagement_GetAllTickets(string status = "", string orderby = "")
        {
            try
            {
                var data = await _orderManagermentService.GetAllTickets(status, orderby);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Contract

        [HttpPost("buyer-contact-upload-signature")]
        public async Task<IActionResult> OrderManagement_BuyerContactUploadSignature([FromBody] OrderManagementApiModel.BuyerContactUploadSignatureModel model)
        {
            try
            {
                var data = await _orderManagermentService.BuyerContactUploadSignature(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("supplier-contact-upload-signature")]
        public async Task<IActionResult> OrderManagement_SupplierContactUploadSignature([FromBody] OrderManagementApiModel.BuyerContactUploadSignatureModel model)
        {
            try
            {
                var data = await _orderManagermentService.SupplierContactUploadSignature(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Expected Shipments

        [HttpGet("get-expected-shipments-by-orderId/{orderId}")]
        public async Task<IActionResult> OrderManagement_GetExpectedShipmentsByOrderId(int orderId)
        {
            try
            {
                var data = await _orderManagermentService.GetExpectedShipmentsByOrderId(orderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Sale order Shipments

        [HttpGet("get-all-shipments")]
        public async Task<IActionResult> OrderManagement_GetAllShipments(int ShipmentTypeId = 0)
        {
            try
            {
                var data = await _orderManagermentService.GetAllShipments(ShipmentTypeId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("sale-order-mark-as-shipped")]
        public async Task<IActionResult> OrderManagement_SaleOrderMarkAsShipped([FromBody] OrderManagementApiModel.MaskAsShippedModel model)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderMarkAsShipped(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("sale-order-mark-as-delivered")]
        public async Task<IActionResult> OrderManagement_SaleOrderMarkAsDelivered([FromBody] OrderManagementApiModel.MaskAsDeliveredModel model)
        {
            try
            {
                var data = await _orderManagermentService.SaleOrderMarkAsDelivered(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("purchase-order-mark-as-shipped")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderMarkAsShipped([FromBody] OrderManagementApiModel.MaskAsShippedModel model)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderMarkAsShipped(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("purchase-order-mark-as-delivered")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderMarkAsDelivered([FromBody] OrderManagementApiModel.MaskAsDeliveredModel model)
        {
            try
            {
                var data = await _orderManagermentService.PurchaseOrderMarkAsDelivered(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-transporters")]
        public async Task<IActionResult> OrderManagement_GetAllTransporters()
        {
            try
            {
                var data = await _orderManagermentService.GetAllTransporters();
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-vehicles-by-transporter/{transporterId}")]
        public async Task<IActionResult> OrderManagement_GetVehiclesByTransporterId(int transporterId)
        {
            try
            {
                var data = await _orderManagermentService.GetVehiclesByTransporterId(transporterId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}