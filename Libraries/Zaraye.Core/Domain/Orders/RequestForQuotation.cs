using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class RequestForQuotation : BaseEntity,StoreEntity,IActiveActivityLogEntity, DefaultColumns, ISoftDeletedEntity
    {
        public int RequestId { get; set; }
        public string CustomRfqNumber { get; set; }
        public decimal Quantity { get; set; }
        public int RfqStatusId { get; set; }

        public int BookerId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public string RejectedReason { get; set; }

        public string Source { get; set; }

        public int StoreId { get; set; }

        public RequestForQuotationStatus RequestForQuotationStatus
        {
            get => (RequestForQuotationStatus)RfqStatusId;
            set => RfqStatusId = (int)value;
        }
    }
}
