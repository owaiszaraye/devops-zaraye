using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.PriceDiscovery
{
    public partial class DailyRate : BaseEntity,StoreEntity, DefaultColumns, ISoftDeletedEntity, IActiveActivityLogEntity
    {
        public int SupplierId { get; set; }
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int AttributeValueId { get; set; }
        public int BrandId { get; set; }
        public decimal Rate { get; set; }
        public bool IncludeGst { get; set; }
        public bool IncludeFirstMile { get; set; }
        public string AttributeXml { get; set; }
        public int DailyRateEnumId { get; set; }

        public bool Published { get; set; }

        public int PublishedById { get; set; }
        public DateTime? PublishedDate { get; set; }
        
        public int RejectedById { get; set; }
        public DateTime? RejectedDate { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }

        public DailyRateEnum DailyRateEnum
        {
            get => (DailyRateEnum)DailyRateEnumId;
            set => DailyRateEnumId = (int)value;
        }
    }
}
