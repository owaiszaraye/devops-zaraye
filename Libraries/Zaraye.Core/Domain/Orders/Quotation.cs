using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Orders
{
    public class Quotation : BaseEntity,StoreEntity, DefaultColumns, ISoftDeletedEntity, IActiveActivityLogEntity
    {
        public int RfqId { get; set; }
        public string CustomQuotationNumber { get; set; }
        public int SupplierId { get; set; }
        public int QuotationStatusId { get; set; }
        public int BrandId { get; set; }
        public decimal QuotationPrice { get; set; }
        public decimal Quantity { get; set; }
        public DateTime PriceValidity { get; set; }
        public int BusinessModelId { get; set; }

        public int BookerId { get; set; }
        public string RejectedReason { get; set; }
        public bool IsApproved { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public string Source { get; set; }

        public int StoreId { get; set; }

        public QuotationStatus QuotationStatus
        {
            get => (QuotationStatus)QuotationStatusId;
            set => QuotationStatusId = (int)value;
        }

        public BusinessModelEnum BusinessModelEnum
        {
            get => (BusinessModelEnum)BusinessModelId;
            set => BusinessModelId = (int)value;
        }
    }
}
