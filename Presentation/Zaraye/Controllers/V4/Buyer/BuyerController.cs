using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Channels;
using Zaraye.Core.Domain.Common;
using Zaraye.Core;
using Zaraye.Models.Api.V4.Buyer;
using Zaraye.Services.Buyer;
using Zaraye.Services.Logging;

namespace Zaraye.Controllers.V4.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/buyer")]
    [AuthorizeApi]
    public class BuyerController : BaseApiController
    {
        #region Fields

        private readonly IBuyerService _buyerService;
        private readonly IAppLoggerService _appLoggerService;

        #endregion

        #region Ctor
        public BuyerController(IBuyerService buyerService, IAppLoggerService appLoggerService)
        {
            _buyerService = buyerService;
            _appLoggerService = appLoggerService;
        }

        #endregion

        #region Methods

        #region Dashboard

        [HttpGet("buyer-dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var data = await _buyerService.GetDashboardData();
                if (data == null)
                    return Ok(new { status = false, Message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        //#region Info

        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            try
            {
                var data = await _buyerService.GetInfo();
                if (data == null)
                    return Ok(new { status = false, Message = "no data found" });
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("info")]
        public async Task<IActionResult> Info([FromBody] BuyerInfoApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });
            try
            {
                var data = await _buyerService.UpdateInfo(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("info-address")]
        public async Task<IActionResult> InfoAddress([FromBody] BuyerInfoAddressApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });
            try
            {
                var data = await _buyerService.UpdateAddress(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //#endregion

        #region Buyer Request

        [HttpGet("buyer-request-history/{active}")]
        public async Task<IActionResult> BuyerRequestHistory(bool active/*List<int> statusIds = null*/)
        {

            try
            {
                var data = await _buyerService.GetBuyerRequestHistory(active);
                if (data.Count <= 0)
                    return Ok(new { success = true, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-request/{requestId}")]
        public async Task<IActionResult> BuyerRequest(int requestId)
        {
            try
            {
                var data = await _buyerService.GetBuyerRequest(requestId);
                if (data == null)
                    return Ok(new { success = true, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("buyer-request-add")]
        public async Task<IActionResult> AddBuyerRequest([FromBody] BuyerRequestApiModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var data = await _buyerService.AddBuyerRequest(model);

                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("buyer-request-edit/{requestId}")]
        public async Task<IActionResult> EditBuyerRequest(int requestId, [FromBody] BuyerRequestApiModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var data = await _buyerService.EditBuyerRequest(requestId, model);

                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Request Quotation History

        [HttpGet("buyer-request-quotations/{requestId}")]
        public async Task<IActionResult> BuyerRequestQuotationHistory(int requestId, List<int> statusIds = null)
        {
            try
            {
                var data = await _buyerService.BuyerRequestQuotationHistory(requestId, statusIds);
                if (data.Count <= 0)
                    return Ok(new { success = true, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("buyer-request-quotation-approved")]
        public async Task<IActionResult> BuyerRequestQuotationApproved([FromBody] BuyerRequestBidApproveApiModel model)
        {
            try
            {
                var data = await _buyerService.BuyerRequestQuotationApproved(model);

                return Ok(new { success = true, message = data });

            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer order

        [HttpGet("buyer-order-list/{active}")]
        public async Task<IActionResult> BuyerOrderList(bool active)
        {
            try
            {
                var data = await _buyerService.BuyerOrderList(active);
                if (data.Count <= 0)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-order-detail/{orderId}")]
        public async Task<IActionResult> BuyerOrderDetail(int orderId)
        {
            try
            {
                var data = await _buyerService.BuyerOrderDetail(orderId);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-order-summary/{orderId}")]
        public async Task<IActionResult> BuyerOrderSummary(int orderId)
        {
            try
            {
                var data = await _buyerService.BuyerOrderSummary(orderId);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //[HttpGet("buyer-order-payment-history")]
        //public async Task<IActionResult> BuyerOrderPaymentHistory()
        //{
        //    var buyer = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
        //        return Ok(new { success = false, message = "Buyer not found" });

        //    var ordersData = new List<BuyerOrderPaymentApiModel>();

        //    var orders = await _orderService.SearchOrdersAsync(customerId: buyer.Id);
        //    foreach (var order in orders)
        //    {
        //        if (order.OrderStatusId != (int)OrderStatus.Cancelled)
        //        {
        //            ordersData.Add(new BuyerOrderPaymentApiModel
        //            {
        //                CustomOrderNumber = order.CustomOrderNumber,
        //                DateFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
        //                Date = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)),
        //                Type = "Debit",
        //                Amount = await _priceFormatter.FormatPriceAsync(order.TotalReceivable)
        //            });
        //        }
        //    }

        //    var orderPayments = await _orderService.GetAllOrderPaymentsAsync(buyerId: buyer.Id, paymentType: "Receivable", isApproved: true);
        //    foreach (var orderPayment in orderPayments)
        //    {
        //        var order = await _orderService.GetOrderByIdAsync(orderPayment.OrderId);
        //        if (order is null)
        //            continue;

        //        ordersData.Add(new BuyerOrderPaymentApiModel
        //        {
        //            CustomOrderNumber = order.CustomOrderNumber,
        //            DateFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(orderPayment.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
        //            Date = (await _dateTimeHelper.ConvertToUserTimeAsync(orderPayment.CreatedOnUtc, DateTimeKind.Utc)),
        //            Type = "Credit",
        //            Amount = await _priceFormatter.FormatPriceAsync(orderPayment.Amount)
        //        });
        //    }

        //    var orderTotal = orders.Where(x => x.OrderStatusId != (int)OrderStatus.Cancelled).Sum(x => x.TotalReceivable);
        //    var orderTotalPaid = orderPayments.Sum(x => x.Amount);
        //    var orderTotalBalance = orderTotal - orderTotalPaid;

        //    try
        //    {
        //        ordersData = ordersData.OrderByDescending(x => x.Date).ToList();
        //        var data = new
        //        {
        //            Orders = ordersData,
        //            Summary = new
        //            {
        //                ReportUrl = _storeContext.GetCurrentStore().Url + "buyerpaymenthistory/",
        //                TotalOutstandingBalance = await _priceFormatter.FormatPriceAsync(orderTotalBalance),
        //            }
        //        };
        //        return Ok(new { success = true, data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        #endregion

        #region Buyer Reports

        [HttpGet("buyer-request-status-summary")]
        public async Task<IActionResult> BuyerRequestStatusSummary()
        {
            try
            {
                var data = await _buyerService.BuyerRequestStatusSummary();
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Support Agent

        [HttpGet("support-agent-info")]
        public async Task<IActionResult> SupportAgentInfo()
        {
            try
            {
                var data = await _buyerService.SupportAgentInfo();
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Ledger

        [HttpGet("buyer-ledger-detail")]
        public async Task<IActionResult> BuyerLedgerDetail(DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            try
            {
                var data = await _buyerService.BuyerLedgerDetail(startDate, endDate, pageSize, pageIndex);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
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
        public async Task<IActionResult> Buyer_BuyerContactUploadSignature([FromBody] BuyerContractUploadSignatureModel model)
        {
            try
            {
                var data = await _buyerService.Buyer_BuyerContactUploadSignature(model);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });

            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Applied Credit Customer

        [HttpPost("apply-for-credit-customer")]
        public async Task<IActionResult> ApplyForCreditCustomer([FromBody] ApplyCreditCustomerModel model)
        {
            try
            {
                await _buyerService.ApplyForCreditCustomer(model);
                return Ok(new { success = true, message = "Record has been added." });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("apply-for-credit-customer")]
        public async Task<IActionResult> GetApplyForCreditCustomer()
        {
            try
            {
                var data = await _buyerService.GetApplyForCreditCustomer();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }


        #endregion

        [HttpPost("credit-application")]
        public async Task<IActionResult> CreditApplication([FromBody] CreditApplicationModel model)
        {

            try
            {
                var data = await _buyerService.CreditApplication(model);
                return Ok(new { success = true, message = data });
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