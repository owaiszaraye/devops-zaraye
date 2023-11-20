using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Custom number formatter
    /// </summary>
    public partial interface ICustomNumberFormatter
    {
        string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest);

        string GenerateOrderCustomNumber(Order order);

        string GenerateRequestCustomNumber(Request request);

        string GeneratePurchaseRequestCustomNumber(Request request);
        
        string GenerateQuotationCustomNumber(Quotation quotation);

        string GenerateRequestForQuotationCustomNumber(RequestForQuotation requestForQuotation);

        string GeneratePaymentCustomNumber(Payment payment);

        string GeneratePOShipmentCustomNumber(Shipment shipment);

        string GenerateSOShipmentCustomNumber(Shipment shipment);
    }
}