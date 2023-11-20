using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Custom number formatter
    /// </summary>
    public partial class CustomNumberFormatter : ICustomNumberFormatter
    {
        #region Fields

        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public CustomNumberFormatter(OrderSettings orderSettings)
        {
            _orderSettings = orderSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate return request custom number
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>Custom number</returns>
        public virtual string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest)
        {
            string customNumber;

            if (string.IsNullOrEmpty(_orderSettings.ReturnRequestNumberMask))
            {
                customNumber = returnRequest.Id.ToString();
            }
            else
            {
                customNumber = _orderSettings.ReturnRequestNumberMask
                    .Replace("{ID}", returnRequest.Id.ToString())
                    .Replace("{YYYY}", returnRequest.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", returnRequest.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", returnRequest.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", returnRequest.CreatedOnUtc.ToString("dd"));

                ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
                ////mask for Id example {#:00000000}
                //var rgx = new Regex(@"{#:\d+}");
                //var match = rgx.Match(customNumber);
                //var maskForReplase = match.Value;
                //
                //rgx = new Regex(@"\d+");
                //match = rgx.Match(maskForReplase);
                //
                //var formatValue = match.Value;
                //if(!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
                //    customNumber = customNumber.Replace(maskForReplase, returnRequest.Id.ToString(formatValue));
                //else
                //    customNumber = customNumber.Insert(0, $"{returnRequest.Id}-");
            }

            return customNumber;
        }

        /// <summary>
        /// Generate order custom number
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Custom number</returns>
        public virtual string GenerateOrderCustomNumber(Order order)
        {
            var customNumber = order.Id.ToString();
            if (order.OrderType == OrderType.SaleOrder)
            {
                if (string.IsNullOrEmpty(_orderSettings.CustomSaleOrderNumberMask))
                    return order.Id.ToString();

                customNumber = _orderSettings.CustomSaleOrderNumberMask
                    .Replace("{ID}", order.Id.ToString())
                    .Replace("{YYYY}", order.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", order.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", order.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", order.CreatedOnUtc.ToString("dd")).Trim();
            }
            if (order.OrderType == OrderType.PurchaseOrder)
            {
                if (string.IsNullOrEmpty(_orderSettings.CustomPurchaseOrderNumberMask))
                    return order.Id.ToString();

                customNumber = _orderSettings.CustomPurchaseOrderNumberMask
                    .Replace("{ID}", order.Id.ToString())
                    .Replace("{YYYY}", order.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", order.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", order.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", order.CreatedOnUtc.ToString("dd")).Trim();
            }
            if (order.OrderType == OrderType.ReturnPurchaseOrder)
            {
                if (string.IsNullOrEmpty(_orderSettings.CustomReturnPurchaseOrderNumberMask))
                    return order.Id.ToString();

                customNumber = _orderSettings.CustomReturnPurchaseOrderNumberMask
                    .Replace("{ID}", order.Id.ToString())
                    .Replace("{YYYY}", order.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", order.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", order.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", order.CreatedOnUtc.ToString("dd")).Trim();
            }
            if (order.OrderType == OrderType.ReturnSaleOrder)
            {
                if (string.IsNullOrEmpty(_orderSettings.CustomReturnSaleOrderNumberMask))
                    return order.Id.ToString();

                customNumber = _orderSettings.CustomReturnSaleOrderNumberMask
                    .Replace("{ID}", order.Id.ToString())
                    .Replace("{YYYY}", order.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", order.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", order.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", order.CreatedOnUtc.ToString("dd")).Trim();
            }

            return customNumber;
        }

        public virtual string GenerateRequestCustomNumber(Request request)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomRequestNumberMask))
                return request.Id.ToString();

            var customNumber = _orderSettings.CustomRequestNumberMask
                .Replace("{ID}", request.Id.ToString())
                .Replace("{YYYY}", request.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", request.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", request.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", request.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        public virtual string GeneratePurchaseRequestCustomNumber(Request request)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomPurchaseRequestNumberMask))
                return request.Id.ToString();

            var customNumber = _orderSettings.CustomPurchaseRequestNumberMask
                .Replace("{ID}", request.Id.ToString())
                .Replace("{YYYY}", request.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", request.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", request.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", request.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        public virtual string GenerateQuotationCustomNumber(Quotation quotation)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomQuotationNumberMask))
                return quotation.Id.ToString();

            var customNumber = _orderSettings.CustomQuotationNumberMask
                .Replace("{ID}", quotation.Id.ToString())
                .Replace("{YYYY}", quotation.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", quotation.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", quotation.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", quotation.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        public virtual string GenerateRequestForQuotationCustomNumber(RequestForQuotation requestForQuotation)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomRfqNumberMask))
                return requestForQuotation.Id.ToString();

            var customNumber = _orderSettings.CustomRfqNumberMask
                .Replace("{ID}", requestForQuotation.Id.ToString())
                .Replace("{YYYY}", requestForQuotation.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", requestForQuotation.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", requestForQuotation.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", requestForQuotation.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        public virtual string GeneratePaymentCustomNumber(Payment payment)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomPaymentNumberMask))
                return payment.Id.ToString();

            var customNumber = _orderSettings.CustomPaymentNumberMask
                .Replace("{ID}", payment.Id.ToString())
                .Replace("{YYYY}", payment.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", payment.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", payment.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", payment.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        public virtual string GeneratePOShipmentCustomNumber(Shipment shipment)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomPOShipmentNumberMask))
                return shipment.Id.ToString();

            var customNumber = _orderSettings.CustomPOShipmentNumberMask
                .Replace("{ID}", shipment.Id.ToString())
                .Replace("{YYYY}", shipment.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", shipment.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", shipment.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", shipment.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        public virtual string GenerateSOShipmentCustomNumber(Shipment shipment)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomSOShipmentNumberMask))
                return shipment.Id.ToString();

            var customNumber = _orderSettings.CustomSOShipmentNumberMask
                .Replace("{ID}", shipment.Id.ToString())
                .Replace("{YYYY}", shipment.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", shipment.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", shipment.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", shipment.CreatedOnUtc.ToString("dd")).Trim();

            return customNumber;
        }

        #endregion
    }
}