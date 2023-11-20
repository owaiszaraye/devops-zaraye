namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class RequestRfqQuotationMapping : BaseEntity
    {
        public int RequestId { get; set; }
        public int RfqId { get; set; }
        public int QuotationId { get; set; }
        public int OrderId { get; set; }
    }
}
